﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderDistFromCamera : MonoBehaviour
{
    public RenderTexture renderTexture;
    public Texture2D tex;
    public Rect rect;

    public int mWidth;
    public int mHeight;
    public float[] distancesFromCamera;

    private bool debugWriteToFile = true;
    private static int debugConter = 0;

    private const float INFINITE_DISTANCE = -1.0f;
    private Shader replacementShader;
    private Camera mCamera;

    private void Start()
    {
        mHeight = mWidth; //it's equal so that two consecutive frames will not overlap each other
        mCamera = GetComponent<Camera>();

        replacementShader = Shader.Find("LidarSensor/Depth");
        if (replacementShader != null)
            mCamera.SetReplacementShader(replacementShader, "");

        tex = new Texture2D(mWidth, mHeight, TextureFormat.RGBAFloat, false);
        renderTexture = new RenderTexture(mWidth, mHeight, 0, RenderTextureFormat.ARGBFloat);
        rect = new Rect(0, 0, mWidth, mHeight);

        distancesFromCamera = new float[mWidth];
    }

    private void RenderCamera()
    {
        // setup render texture
        var currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        mCamera.targetTexture = renderTexture;

        // Render the camera's view.
        mCamera.Render();

        // Set texture2D
        tex.ReadPixels(rect, 0, 0);
        tex.Apply();

        // post-render
        RenderTexture.active = currentRT;
    }

    private void CalculateDistances()
    {
        float[] buffer = tex.GetRawTextureData<float>().ToArray();
        const int numOfFloatsInPixel = 4; //fragment shader returns float4
        int numOfFloatsInWidth = numOfFloatsInPixel * mWidth;

        int counter;
        float sumDistances;
        int arrIndex = 0;
        for (int x = 0; x < numOfFloatsInWidth; x += numOfFloatsInPixel)
        {
            counter = 0;
            sumDistances = 0;
            for (int y = 0; y < mHeight; y++)
            {
                int index = x + (y * numOfFloatsInWidth);
                float rVal = buffer[index]; //fragment shader returns float4(dist, dist, dist, dist) so we can read the distance from any of the 4 components
                if (rVal >= 0) //if rVal<0 it means that it is background (distance is infinite)
                {
                    counter++;
                    sumDistances += rVal;
                }
            }
            if (counter > 0)
            {
                float averageDistance = sumDistances / counter;
                distancesFromCamera[arrIndex] = averageDistance;
            }
            else
            {
                distancesFromCamera[arrIndex] = INFINITE_DISTANCE;
            }
            arrIndex++;
        }
    }

    private void DebugWriteTextureToFile()
    {
        //this function is used for debugging. you can use matlab code script_read_image_from_csv.m to see the Texture2D frames of each camera
        const string folderPath = "Debug_Images";
        if (debugConter == 1)
        {
            bool isFolderExists = System.IO.Directory.Exists(folderPath);
            if (isFolderExists)
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderPath);

                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            System.IO.Directory.CreateDirectory(folderPath);
        }

        //This function is used to read the texture in matlab so we can see the Texture2D
        float[] buffer = tex.GetRawTextureData<float>().ToArray();
        string filePath = folderPath + "/matlabImage_" + debugConter.ToString() + ".csv";
        System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
        int numOfFloatsInWidth = 4 * mWidth;
        for (int j = mHeight - 1; j >= 0; j--) //reversing j since the the Y axis in the image is upside down
        {
            for (int i = 0; i < numOfFloatsInWidth; i += 4)
            {
                int index = (j * numOfFloatsInWidth) + i;
                float rVal = buffer[index];
                if (i == numOfFloatsInWidth - 4)
                    writer.Write(rVal);
                else
                    writer.Write(rVal + ",");
            }
            writer.Write(System.Environment.NewLine);
        }
        writer.Close();
    }

    private void Update()
    {
        RenderCamera();
        CalculateDistances();

        if (debugWriteToFile)
        {
            debugConter++;
            DebugWriteTextureToFile();
            debugWriteToFile = false;
        }
    }

    private void OnDestory()
    {
    #if UNITY_EDITOR
        DestroyImmediate(renderTexture);
        DestroyImmediate(tex);
    #else
        Destroy(renderTexture);
        Destroy(tex);
    #endif
    }
}
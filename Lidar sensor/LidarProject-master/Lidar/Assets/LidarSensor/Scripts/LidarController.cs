using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarController : MonoBehaviour
{
    private const float CAMERA_FOV_DEG = 15f;
    private const float CAMERA_FOV_RAD = CAMERA_FOV_DEG * Mathf.Deg2Rad;
    private const float CAMERA_RANGE_MIN = 0.2f;
    private const float CAMERA_RANGE_MAX = 30f;
    private Color INFINITE_DISTANCE_COLOR = new Color(-1.0f, -1.0f, -1.0f, -1.0f);

    // 
    public GameObject CamRange;
    public float camerasRadius;
    public int numOfCameras;


    private int numOfLidarSimulatedCameras = 920;

    //
    private List<Camera> m_camArray;
    public float[] distancesFromAllSensors;
    int cameraWidth;
    private bool debugMovement = true;
    private float movement;
    private void SetupCamera(Camera cam, int target)
    {
        // Update camera setup
        //cam.enabled = false;

        // Update generic data
        cam.fieldOfView = CAMERA_FOV_DEG;
        cam.nearClipPlane = CAMERA_RANGE_MIN - 0.05F;
        cam.farClipPlane = CAMERA_RANGE_MAX + 0.05f;
        //cam.enabled = true;
        //if (target < Display.displays.Length)
        cam.targetDisplay = target;

        // Force camera to render depth buffer
        // Unity disables it by default for faster renders
        cam.depthTextureMode |= DepthTextureMode.Depth;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = INFINITE_DISTANCE_COLOR;
        cam.fieldOfView = 360.0f / numOfCameras;

        //Add post processing shader which renders depth images
        cameraWidth = numOfLidarSimulatedCameras / numOfCameras;
        //cameraWidth = 500;


        cam.gameObject.AddComponent<RenderDistFromCamera>().mWidth = cameraWidth;
        //Tried to define  "renderDistFromCamera=cam.gameObject.AddComponent<RenderDistFromCamera>()" and then to call renderDistFromCamera.someVariable = value
        //The values of the variables did not pass to the class RenderDistFromCamera. so I assigned a single variable mWidth in AddComponent API. maybe I need to use struct in order to assign multiple variables?
    }

    private void CreateCameraArray()
    {
        for (int i = 0; i < numOfCameras; ++i)
        {
            // calculate position and rotation
            float rad = i * (2 * Mathf.PI / numOfCameras);
            Vector3 pos = new Vector3(Mathf.Sin(rad) * camerasRadius,
                                    0,
                                    Mathf.Cos(rad) * camerasRadius);
            Quaternion rot = Quaternion.Euler(0, i * (360f / numOfCameras), 0);

            // Initialize camera gameobject
            GameObject obj = new GameObject();
            obj.transform.parent = CamRange.transform;
            obj.transform.localPosition = pos;
            obj.transform.localRotation = rot;

            // Add camera
            Camera cam = obj.AddComponent<Camera>();
            m_camArray.Add(cam);

            SetupCamera(cam, i+1);
        }
    }

    private void Start()
    {
        if (debugMovement)
            movement = 1.0f;
        distancesFromAllSensors = new float[numOfLidarSimulatedCameras];
        m_camArray = new List<Camera>();
        CreateCameraArray();
    }

    private void Update()
    {
        if (debugMovement)
        {
            movement = -movement;
            transform.parent.transform.position += new Vector3(movement, movement, movement);
        }
    }

    private void LateUpdate()
    {
        //calling it after RenderDistFromCamera.Update()
        //distancesFromAllSensors
        //RenderDistFromCamera renderDistFromCamera = 

        int index = 0;
        int centerWidth = (cameraWidth - 1) / 2;
        Camera firstCamera = m_camArray[0];
        RenderDistFromCamera firstRenderDistFromCamera = firstCamera.GetComponent<RenderDistFromCamera>();
        for (int j = centerWidth; j < cameraWidth; j++)
        {
            distancesFromAllSensors[index] = firstRenderDistFromCamera.distancesFromCamera[j];
            index++;
        }

        for (int i = 1; i < numOfCameras; i++)
        {
            Camera cam = m_camArray[i];
            RenderDistFromCamera renderDistFromCamera = cam.GetComponent<RenderDistFromCamera>();
            for (int j = 0; j < cameraWidth; j++)
            {
                distancesFromAllSensors[index] = renderDistFromCamera.distancesFromCamera[j];
                index++;
            }
        }

        for (int j = 0; j <= centerWidth - 1; j++)
        {
            distancesFromAllSensors[index] = firstRenderDistFromCamera.distancesFromCamera[j];
            index++;
        }
    }
}

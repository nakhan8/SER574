using System.Collections;
using System.Collections.Generic;
using System.Linq;  
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    public Vector3 center;
    public GameObject hawkPrefab;
    public GameObject dovePrefab;
    public GameObject foodPrefab;

    public int min;
    public int max;

    public TMP_InputField hawkNum;
    public TMP_InputField doveNum;
    public TMP_InputField foodNum;
    public TMP_Dropdown population;

    public TMP_InputField foodValue;
    public TMP_InputField injury;
    public TMP_InputField bluffing;
    public TMP_InputField baseReq;

    public Button btn_Start;
    public Button btn_Next;
    public Button btn_Stop;

    public int foodValueInt;
    public int injuryInt;
    public int bluffingInt;
    public int baseReqInt;

    private string csvFileName;
    private StringBuilder sb;

    private DD_DataDiagram m_DataDiagram;
    List<GameObject> lineList = new List<GameObject>();
    private float h = 0;

    public int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject dd = GameObject.Find("DataDiagram");
        if (null == dd)
        {
            Debug.LogWarning("can not find a gameobject of DataDiagram");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        AddALine();
        AddALine();
        AddALine();

        btn_Next.enabled = false;
        btn_Stop.enabled = false;

        sb = new StringBuilder();
    }

    private bool updateSwitch = false;
    private int updateIndex = 0;
    // Update is called once per frame
    void Update()
    {
        if (updateSwitch)
        {
            if (updateIndex < 100)
            {
                Btn_Next_Click();
                updateIndex++;
            }
            if (updateIndex == 100)
            {
                updateSwitch = false;
                updateIndex = 0;
            }
        }
    }

    public void Btn_Start_Click()
    {
        btn_Next.enabled = true;
        btn_Stop.enabled = true;
        btn_Start.enabled = false;

        GameObject hawk = GameObject.Find("Hawk(Clone)");
        while(hawk != null)
        {
            GameObject.DestroyImmediate(hawk);
            hawk = GameObject.Find("Hawk(Clone)");
        }

        GameObject dove = GameObject.Find("Dove(Clone)");
        while (dove != null)
        {
            GameObject.DestroyImmediate(dove);
            dove = GameObject.Find("Dove(Clone)");
        }

        GameObject food = GameObject.Find("Food(Clone)");
        while (food != null)
        {
            GameObject.DestroyImmediate(food);
            food = GameObject.Find("Food(Clone)");
        }


        for (int i = 0; i < System.Convert.ToInt32(hawkNum.text); i++)
            spawnPrefab(hawkPrefab);
            
        for(int i = 0; i < System.Convert.ToInt32(doveNum.text); i++)
            spawnPrefab(dovePrefab);

        for (int i = 0; i < System.Convert.ToInt32(foodNum.text); i++)
            spawnPrefab(foodPrefab);

        hawkNum.enabled = false;
        doveNum.enabled = false;
        foodNum.enabled = false;
        foodValue.enabled = false;
        injury.enabled = false;
        bluffing.enabled = false;
        baseReq.enabled = false;

        foodValueInt = System.Convert.ToInt32(foodValue.text);
        injuryInt = System.Convert.ToInt32(injury.text);
        bluffingInt = System.Convert.ToInt32(bluffing.text);
        baseReqInt = System.Convert.ToInt32(baseReq.text);

        sb.Clear();

        sb.Append("Howk").Append(",");
        sb.Append("Dove").Append(",");
        sb.Append("Total").Append(",");

        sb.Append(hawkNum.text).Append(",");
        sb.Append(doveNum.text).Append(",");
        sb.Append((System.Convert.ToInt32(hawkNum.text) + System.Convert.ToInt32(doveNum.text)).ToString()).Append(",");

    }

    public GameObject spawnPrefab(GameObject prefab)
    {
        Vector3 pos = center + new Vector3(Random.Range(min, max), Random.Range(min, max), 0);

        return Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    public void Btn_Next_Click()
    {
        BroadcastMessage("Expiring");
        for (int i = 0; i < System.Convert.ToInt32(foodNum.text); i++)
            spawnPrefab(foodPrefab);

        List<GameObject> foods = new List<GameObject>();
        List<GameObject> hawks = new List<GameObject>();
        List<GameObject> doves = new List<GameObject>();
        List<GameObject> agents = new List<GameObject>();
        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == "Food(Clone)")
            {
                foods.Add(go);
            }
            else if (go.name == "Hawk(Clone)")
            {
                hawks.Add(go);
                agents.Add(go);
            }
            else if (go.name == "Dove(Clone)")
            {
                doves.Add(go);
                agents.Add(go);
            }
        }
        while (agents.Count > 0)
        {
            if (foods.Count > 0)
            {
                if (agents.Count > 1)
                {
                    int random1 = Random.Range(0, agents.Count - 1);
                    GameObject agent1 = agents[random1];
                    agents.RemoveAt(random1);
                    int random2 = Random.Range(0, agents.Count - 1);
                    GameObject agent2 = agents[random2];
                    agents.RemoveAt(random2);
                    if (hawks.Contains(agent1) && hawks.Contains(agent2))
                    {
                        HawkManager hm = agent1.transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                        HawkManager hm2 = agent2.transform.GetComponent<HawkManager>();
                        hm2.energy -= (injuryInt + baseReqInt);
                    }
                    else if (doves.Contains(agent1) && doves.Contains(agent2))
                    {
                        DoveManager dm = agent1.transform.GetComponent<DoveManager>();
                        dm.energy += (foodValueInt - bluffingInt - baseReqInt);
                        DoveManager dm2 = agent2.transform.GetComponent<DoveManager>();
                        dm2.energy -= (bluffingInt + baseReqInt);
                    }
                    else if (hawks.Contains(agent1) && doves.Contains(agent2))
                    {
                        HawkManager hm = agent1.transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                        DoveManager dm = agent2.transform.GetComponent<DoveManager>();
                        dm.energy -= baseReqInt;
                    }
                    else
                    {
                        HawkManager hm = agent2.transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                        DoveManager dm = agent1.transform.GetComponent<DoveManager>();
                        dm.energy -= baseReqInt;
                    }
                }
                else
                {
                    if (hawks.Contains(agents[0]))
                    {
                        HawkManager hm = agents[0].transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                    }
                    else
                    {
                        DoveManager dm = agents[0].transform.GetComponent<DoveManager>();
                        dm.energy += (foodValueInt - baseReqInt);
                    }
                    agents.RemoveAt(0);
                }
                int random3 = Random.Range(0, foods.Count - 1);
                GameObject.DestroyImmediate(foods[random3]);
                foods.RemoveAt(0);
            }
            else
            {
                foreach(GameObject go in agents)
                {
                    if (hawks.Contains(go))
                    {
                        HawkManager hm = go.transform.GetComponent<HawkManager>();
                        hm.energy -= baseReqInt;
                    }
                    else
                    {
                        DoveManager dm = go.transform.GetComponent<DoveManager>();
                        dm.energy -= baseReqInt;
                    }
                }
                break;
            }
                
        }

        int howkCount = 0;
        int doveCount = 0;

        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == "Hawk(Clone)")
            {
                howkCount++;
            }
            else if (go.name == "Dove(Clone)")
            {
                doveCount++;
            }
        }

        Debug.Log(howkCount + "; " + doveCount);

        if (null == m_DataDiagram)
            return;

        int j = 0;
        foreach (GameObject l in lineList)
        {
            if (j == 0)
            {
                m_DataDiagram.InputPoint(l, new Vector2(1,(float)(howkCount/10.0)));
            }
            else if(j == 1)
            {
                m_DataDiagram.InputPoint(l, new Vector2(1, (float)(doveCount / 10.0)));
            }
            else
            {
                m_DataDiagram.InputPoint(l, new Vector2(1, (float)((howkCount+doveCount) / 10.0)));
            }
            j++;
        }

        sb.Append(howkCount.ToString()).Append(",");
        sb.Append(doveCount.ToString()).Append(",");
        sb.Append((howkCount + doveCount).ToString()).Append(",");
    }

    public void Btn_Complete_Click()
    {
        Btn_Start_Click();
        updateSwitch = true;
    }


    public void Btn_Stop_Click()
    {
        hawkNum.enabled = true;
        doveNum.enabled = true;
        foodNum.enabled = true;
        foodValue.enabled = true;
        injury.enabled = true;
        bluffing.enabled = true;
        baseReq.enabled = true;

        btn_Next.enabled = false;
        btn_Stop.enabled = false;
        btn_Start.enabled = true;

        SaveFileDlg pth = new SaveFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "All files (*.*)|*.*";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;
        pth.title = "Save Project";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (SaveFileDialog.GetSaveFileName(pth))
        {
            string filepath = pth.file;
            Debug.Log(filepath);

            StreamWriter sw = new StreamWriter(filepath);


            string result = sb.ToString().Substring(0, sb.ToString().Length - 1);
            string[] s = result.Split(',');
            for (int i = 0; i < s.Count() / 3; i++)
            {
                sw.WriteLine(s[i * 3] + "," + s[i * 3 + 1] + "," + s[i * 3 + 2]);
            }

            sw.Flush();
            sw.Close();
        }
    }

    public void reproduction(string agent, int energy)
    {
        if (agent == "Hawk")
        {
            HawkManager hm = spawnPrefab(hawkPrefab).GetComponent<HawkManager>();
            hm.energy = energy / 2;
        }
        else
        {
            DoveManager dm = spawnPrefab(dovePrefab).GetComponent<DoveManager>();
            dm.energy = energy / 2;
        }

    }

    void AddALine()
    {

        if (null == m_DataDiagram)
            return;

        Color color = Color.HSVToRGB((h += 0.1f) > 1 ? (h - 1) : h, 0.8f, 0.8f);
        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);
    }

    public void Quit()
    {
        Application.Quit();
    }

}

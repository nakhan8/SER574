using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HawkManager : MonoBehaviour
{
    public TMP_InputField death;
    public TMP_InputField reproduction;

    public int energy;
    // Start is called before the first frame update
    void Start()
    {
        energy = 100;
        death = GameObject.Find("DeathInput").GetComponent<TMP_InputField>();
        reproduction = GameObject.Find("ReproductionInput").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if(energy <= System.Convert.ToInt32(death.text))
            GameObject.DestroyImmediate(this.gameObject);
        if(energy >= System.Convert.ToInt32(reproduction.text))
        {
            GameObject.Find("Canvas").GetComponent<GameManager>().reproduction("Hawk", energy);

            energy /= 2;
        }
            
    }

}

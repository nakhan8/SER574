using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomGenerator : MonoBehaviour
{

    public InputField HawkInputField, DoveInputField, FoodInputField, EnergyValueFoodInputField, EnergyLossInjuryInputField, DeathThresholdInputField, ReproductionThresholdInputField, FoodExpirationTimeInputField;
    //public Text HawkText, DoveText, FoodText, EnergyValueFoodText, EnergyLossInjuryText, DeathThresholdText;



    //public Vector3 center;
    public GameObject dove;
    public GameObject hawk;
    public GameObject food;

    private Vector3 Min;
    private Vector3 Max;
    private bool Updatevar = false;

    private float _xAxis;
    private float _yAxis;
    private float _zAxis; //If you need this, use it
    private Vector3 _randomPosition;
    public bool _canInstantiate;

    private void Start()
    {

        SetRanges();


    }


    public void onQuit()
    {
        // UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }



    private void Update()
    {
        if (Updatevar == true)
        {
            InstantiateRandomObjects(dove);
            InstantiateRandomObjects(hawk);
            InstantiateRandomObjects(food);
        }

    }



    public void onStart()
    {
        for (int i = 0; i < int.Parse(DoveInputField.text); i++)
        {
            InstantiateRandomObjects(dove);
        }

        for (int i = 0; i < int.Parse(HawkInputField.text); i++)
        {
            InstantiateRandomObjects(hawk);

        }
        for (int i = 0; i < int.Parse(FoodInputField.text); i++)
        {
            InstantiateRandomObjects(food);

        }
        Updatevar = true;
    }

    public void onStop()
    {
        Destroy(this.gameObject);
        gameObject.SetActive(false);
    }
    //Here put the ranges where your object will appear, or put it in the inspector.

   
    private void SetRanges()
    {
        Min = new Vector3(0, 0, 0); //Random value.
        Max = new Vector3(12, 5, 0); //Another ramdon value, just for the example.
    }
    private void InstantiateRandomObjects(GameObject gameObject)
    {
        _xAxis = UnityEngine.Random.Range(Min.x, Max.x);
        _yAxis = UnityEngine.Random.Range(Min.y, Max.y);
        _zAxis = 0;
        _randomPosition = new Vector3(_xAxis, _yAxis, _zAxis);
        if (_canInstantiate)
        {
            Instantiate(gameObject, _randomPosition, Quaternion.identity);
        }

    }
}
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    [SerializeField]
    Text InputField;
    string inputString="";
    int i = 0;
    int result;
    string symbol = "";
    int[] number = new int[2];
    bool dres = false;


    public void buttonPressed()
    {
        if (dres)
        {
            InputField.text="";
            inputString = "";
            dres = false;
        }
        string buttonValue= EventSystem.current.currentSelectedGameObject.name;
        inputString += buttonValue;
        int arg;
        if(int.TryParse(buttonValue, out arg))
        {
            if (i > 1) i = 0;
            number[i] = arg;
            i++;
        }
        else
        {
            switch (buttonValue)
            {
                case "+":
                    symbol = buttonValue;
                    break;
                    
                case "-":
                    symbol = buttonValue;
                    break;

                case "=":
                    switch (symbol)
                    {
                        case "+":
                            result = number[0] + number[1];
                            break;
                        case "-":
                            result = number[0] - number[1];
                            break;
                        default:
                           break;
                    }
                    dres = true;
                    inputString = result.ToString();
                    number = new int[2];
                    break;
            }
        }


        InputField.text = inputString;
    }
}

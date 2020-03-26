using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodManager : MonoBehaviour
{

    public TMP_InputField expiration;

    public int expiredTime;
    // Start is called before the first frame update
    void Start()
    {
        expiration = GameObject.Find("ExpirationInput").GetComponent<TMP_InputField>();

        expiredTime = System.Convert.ToInt32(expiration.text);
    }

    // Update is called once per frame
    void Update()
    {
        if(expiredTime == 0)
            GameObject.DestroyImmediate(this.gameObject);
    }

    public void Expiring()
    {
        expiredTime--;
    }
}

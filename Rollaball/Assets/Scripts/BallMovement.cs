using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallMovement : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    private int countPoints;
    public Text Score;
    public Text WinText;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        countPoints = 0;
        Score.text  = "Score: "+countPoints.ToString();
        WinText.text = "";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(move*speed);
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Pick Up")){
            other.gameObject.SetActive(false);
            countPoints += 1;
            Score.text  = "Score: "+countPoints.ToString();
        }

        if(countPoints == 6)
            WinText.text  = "You Win!";
    }
}

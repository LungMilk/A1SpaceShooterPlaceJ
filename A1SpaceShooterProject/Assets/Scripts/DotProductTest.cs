using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotProductTest : MonoBehaviour
{
    //script here from package
    //it converts a given angle into a position relative to the worlds origin.
    public float redAngle;
    public float blueAngle;

    // Update is called once per frame
    void Update()
    {
        //convertion to radians for math sake
        float redRads = redAngle * Mathf.Deg2Rad;
        float blueRads = blueAngle * Mathf.Deg2Rad;
        //trig to plot it into a triangle
        Vector3 redVector = new Vector3(Mathf.Cos(redRads), Mathf.Sin(redRads), 0);
        Vector3 blueVector = new Vector3(Mathf.Cos(blueRads), Mathf.Sin(blueRads), 0);

        Debug.DrawLine(Vector3.zero, redVector, Color.red);
        Debug.DrawLine(Vector3.zero, blueVector, Color.blue);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //gives the dot product of the vectors which looks to be the pythag theorem.
            float dotProduct = redVector.x * blueVector.x + redVector.y * blueVector.y;
            //checking if the value is greater that the epsilon shape preventing it from going beyond a rotation?
            if (Mathf.Abs(dotProduct) < float.Epsilon) dotProduct = 0;
            
            print($"<color=yellow><size=18>{dotProduct}</size></color>");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public float angularSpeed = 45f;
    public float targetAngle = 130f;

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.green);
        
        float currentRotation = transform.rotation.eulerAngles.z;
        currentRotation = StandardizeAngle(currentRotation);
        //Debug.Log($"<color=yellow><size=18>{currentRotation}</size></color>");
        
        if (targetAngle - currentRotation < 0)
        {
            if (currentRotation > targetAngle)
                transform.Rotate(0, 0, -angularSpeed * Time.deltaTime);
        }
        else
        {
            if (currentRotation < targetAngle)
                transform.Rotate(0, 0, angularSpeed * Time.deltaTime);
        }
    }

    public float StandardizeAngle(float inAngle)
    {
        inAngle %= 360;

        if (inAngle > 180)
        {
            inAngle -= 360;
        }

        return inAngle;
    }
}

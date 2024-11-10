using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : MonoBehaviour
{
    Vector3 pos;
    Quaternion quat;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,5);
    }

    // Update is called once per frame
    void Update()
    {
        //need to have it add to its local position what straight means so it travels in a straight path
        //transform.GetLocalPositionAndRotation(out pos, out quat);
        //pos.x += 10 * Time.deltaTime;
        //transform.SetLocalPositionAndRotation(pos, quat);
        transform.Translate(Vector3.up * 10 * Time.deltaTime,Space.Self);
        
    }
}

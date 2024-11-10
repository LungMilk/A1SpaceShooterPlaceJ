using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public GameObject target;

    public float sightDistance;
    public float visionAngle;

    // Update is called once per frame
    void Update()
    {
        float angleInRads = visionAngle * Mathf.Deg2Rad;

        Vector3 leftVector = new Vector3(Mathf.Cos(angleInRads), Mathf.Sin(angleInRads), 0) * sightDistance;
        Vector3 rightVector = new Vector3(Mathf.Cos(-angleInRads), Mathf.Sin(-angleInRads), 0) * sightDistance;

        Debug.DrawLine(transform.position, transform.position + leftVector, Color.cyan);
        Debug.DrawLine(transform.position, transform.position + rightVector, Color.cyan);

        if (target != null)
        {
            Vector3 vectorToTarget = target.transform.position - transform.position;

            Debug.DrawLine(transform.position, transform.position + vectorToTarget, Color.green);

            float targetDotProduct = Vector3.Dot(transform.right, vectorToTarget.normalized);
            float visionDotProduct = Vector3.Dot(transform.right, leftVector.normalized);

            if (targetDotProduct >= visionDotProduct && vectorToTarget.magnitude <= sightDistance)
            {
                print("<color=orange><size=18>Target spotted!</size></color>");
            }
        }
    }
}

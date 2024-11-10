using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public List<Transform> asteroidTransforms;
    public Transform enemyTransform;
    public GameObject enemy;
    public GameObject missle;

    public GameObject bombPrefab;
    public Transform bombsTransform;

    public List<Vector3> points;

    public List<int> IsPointOccupied;

    public float accelerationTime = 1f;
    public float decelerationTime = 1f;
    public float maxSpeed = 7.5f;
    public float turnSpeed = 180f;

    public int numPoints;
    public float asteroidDetectionDistance;

    private float acceleration;
    private float deceleration;
    private Vector3 currentVelocity;
    private float maxSpeedSqr;

    public int shotBurst;
    public float burstInterval;
    public int collectionLimit;

    public bool isShooting = false;
    public bool isCollecting = false;

    private void Start()
    {
        
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        maxSpeedSqr = maxSpeed * maxSpeed;


    }

    void Update()
    {
        PlayerMovement();
        //i could have a global class handle all proximity and pass it that as it would manage everything
        if (Input.GetKey(KeyCode.R))
        {
            Circle(5,1.5f,Color.blue,1);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isShooting)
        {
            StartCoroutine(BroadsideMissles(shotBurst, burstInterval));
        }

        if (Input.GetKey(KeyCode.Space))
        {
            //shield
            //shield is broken and not in half
            Circle(numPoints, 2, Color.green, 2);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            collectionLimit = 0;
           
            //also not consistent in detecting a bool press
            //asteroid collection
            isCollecting = !isCollecting;
        }
        if (isCollecting)
        {
            AsteroidCollection();
        }

        for (int i = 0; i < points.Count; i++)
        {
            print($"what is in is point occupided index: {IsPointOccupied[i]}");
            if (IsPointOccupied[i] != 0)
            {
                asteroidTransforms[IsPointOccupied[i]].position = points[i];
            }
        }
        
    }
    void PlayerMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.S))
            moveDirection += Vector3.down;

        if (Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;
        if (Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;

        if (moveDirection.sqrMagnitude > 0)
        {
            currentVelocity += Time.deltaTime * acceleration * moveDirection;
            if (currentVelocity.sqrMagnitude > maxSpeedSqr)
            {
                currentVelocity = currentVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            Vector3 velocityDelta = Time.deltaTime * deceleration * currentVelocity.normalized;
            if (velocityDelta.sqrMagnitude > currentVelocity.sqrMagnitude)
            {
                currentVelocity = Vector3.zero;
            }
            else
            {
                currentVelocity -= velocityDelta;
            }
        }

        transform.position += currentVelocity * Time.deltaTime;
    }
    IEnumerator BroadsideMissles(int numOfMissles,float ShotInterval)
    {
        for (int i = 0; i < numOfMissles; i++)
        {
            isShooting = true;
            Instantiate(missle, transform.position, MisslesDirection());
            yield return new WaitForSeconds(ShotInterval);
        }
        isShooting = false;
    }
    Quaternion MisslesDirection()
    {
        //missles can have their own script that moves them when spawned at a constant rate forward, all this need to do is rotate them to a point
        //z axis is forward upward in cross product of forward and upward and Y is the crossproduct between z and x
        //need direction we need to turn it
        //give it a location
        //angle is the tangent between the x and y giving a float to 
        //quaternion rotation .angleAxis angle - 90) 

        //the direction must be inverting depending on the axis so the values should be converted
        //not entirely true because it also changes depending on the positoin, maybe we should limit to detecting some rotation on the unit circle

        //miisles will now get the direction to the mousepostoworldpoint
        // I was probably missing - transfor position to keep it local for direction.
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle -90, Vector3.forward);
        return rotation;
    }

    void AsteroidCollection()
    {
        //can i not increase a for loops limit while it runs
        //i need it to somehow ignore an asteroid that has already been caught
        
        for (int i = 0; i < asteroidTransforms.Count; i++)
        {
            for(int j = 0;j<IsPointOccupied.Count;j++)
            {
                if (IsPointOccupied[j] == i) 
                {
                    break; 
                }
            }
            if (collectionLimit >3) { break; }
            Color radarColor = Color.white;
            Vector3 Target = asteroidTransforms[i].position - transform.position;
            float magnitude = Mathf.Sqrt((Target.x * Target.x) + (Target.y * Target.y));

            Debug.DrawLine(transform.position, asteroidTransforms[i].position, radarColor);
            if (magnitude < asteroidDetectionDistance)
            {
                //I need to delay this so it checks it once and ignores said asteroid
                
                radarColor = Color.red;
                //this will retain the index of the detectd asteroid
                i = IsPointOccupied[checkAvailableSpaces()];
                asteroidTransforms[i].position = points[collectionLimit];
                collectionLimit++;
            }
        }
        Circle(3, 1.5f, Color.blue, 1);

    }
    //have a method that goes through a list and returns the first available space 
    public int checkAvailableSpaces()
    {
        for (int i = 0;i < IsPointOccupied.Count;i++)
        {
            if (IsPointOccupied[i] ==0) { print($" not occupied point{i}"); return i; }
        }
        return -1;
        //if(checkAvailableSpaces() == -1) { for (int j = 0; j < IsPointOccupied.Count; j++) { IsPointOccupied[i] = false; } }
    }

    void EnemyDetection(GameObject IncomingTarget)
    {
        //If the enemy is within a certain distance to the "active" sheild points it it detected.
        //Debug.DrawLine(enemyTransform.position - transform.position + transform.position,transform.position);
        //get the magnitude of two points so pythag

        // i want to use this method for the active points in the shield
        for (int i = 0; i < points.Count / 2 +1; i++)
        {
            //reliant on its restrictive nature as the object strill needs to be specifically referenced.
            Vector3 TargetCollider = IncomingTarget.transform.position - points[i];
            float magnitude = Mathf.Sqrt((TargetCollider.x * TargetCollider.x) + (TargetCollider.y * TargetCollider.y));
            if (magnitude < 0.5)
            {
                print("touching" + i);
                Destroy(IncomingTarget);
            }
        }

    }
    //changed parameters to make use of a single method to draw multiple shapes, versatility
    void Circle(int steps,float radius, Color color,int size)
    {
        //how do i rotate where the points are being generated
        // creates a shape equal to the amount of points given
        for (int i = 0; i < points.Count; i++)
        {
            //checks the iterations of the loop to the total of steps giving a fraction like 2 loop iteraiton / 5 total 
            float circumferenceProgess = (float)i / steps;
            //using the progress fraction it multiplies it by pi and 2 mapping it around the unit circle, multiplying it by two allows it to circle around for a full shape
            float currentRadian = circumferenceProgess * 2 * Mathf.PI;

            //taking the cos and sin value gives the relative distanc of the adjacent vector and opossing vector which together they plot the final point
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            //broke it?
            //points.Add(new Vector3(x, y, 0) + transform.position);
            points[i] = new Vector3(x, y, 0) + transform.position;
        } 

        //when it draws it is indexing out of range for some reason

        //to limit it to the front only I simply have the count restricted so it doesnt draw to the end points
        for (int i = 0;i < points.Count; i++)
        {
            Debug.DrawLine(points[i], points[i+1], color);

            //makes it a complete shape
            if (i > points.Count) { i = 0; }
        }
    }
}

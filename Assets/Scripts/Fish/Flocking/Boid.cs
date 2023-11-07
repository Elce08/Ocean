using System.Diagnostics;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private const float MAX_SPEED = 2f;
    private const float MAX_FORCE = 0.05f;

    private Vector3 velocity;
    private Vector3 location;

    [Header("Flock Settings")]
    [Range(1, 150)] public int flockNum = 2;
    [Range(0, 5000)] public int fragmentedFlock = 30;
    [Range(0, 1)] public float fragmentedFlockYLimit = 0.5f;
    [Range(0, 1.0f)] public float migrationFrequency = 0.1f;
    [Range(0, 1.0f)] public float posChangeFrequency = 0.5f;
    [Range(0, 100)] public float smoothChFrequency = 0.5f;


    [Header("Fish Settings")]
    public GameObject fishPref;
    [Range(1, 9999)] public int fishNum = 100;
    [Range(0, 150)] public float fishSpeed = 1.0f;
    [Range(0, 100)] public int fragmentedFish = 10;
    [Range(0, 1)] public float fragmentedFishYLimit = 1;
    [Range(0, 10)] public float soaring = 0.5f;
    public Vector2 scaleRandom = new Vector2(1.0f, 1.5f);

    Transform[] fishTransform, flocksTransform;
    float[] fishsSpeed, fishsSpeedCur, spVelocity;
    Vector3[] rdTargetPos, flockPos, velFlocks;
    int[] curentFlock;
    Transform thisTransform;
    public bool debug;

    private void Awake()
    {
        thisTransform = transform;
    }
    private void Start()
    {
        location = new Vector3(Random.Range(0f, Screen.width), Random.Range(0f, Screen.height), 0f);
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized * MAX_SPEED;
        CreateFlock();
        CreateFish();
    }

    private void Update()
    {
        Vector3[] boidLocations = Flock();
        Vector3 acceleration = Cohere(boidLocations);

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, MAX_SPEED);

        location += velocity * Time.deltaTime;
        WrapIfNeeded();
        RenderBoid();
    }

    void CreateFish()
    {
        fishTransform = new Transform[fishNum];
        fishsSpeed = new float[fishNum];
        fishsSpeedCur = new float[fishNum];
        rdTargetPos = new Vector3[fishNum];
        spVelocity = new float[fishNum];

        for (int b = 0; b < fishNum; b++)
        {
            fishTransform[b] = Instantiate(fishPref, thisTransform).transform;
            Vector3 lpv = Random.insideUnitSphere * fragmentedFish;
            fishTransform[b].localPosition = rdTargetPos[b] = new Vector3(lpv.x, lpv.y * fragmentedFishYLimit, lpv.z);
            fishTransform[b].localScale = Vector3.one * Random.Range(scaleRandom.x, scaleRandom.y);
            fishTransform[b].localRotation = Quaternion.Euler(0, Random.value * 360, 0);
            curentFlock[b] = Random.Range(0, flockNum);
            fishsSpeed[b] = Random.Range(3.0f, 7.0f);
        }
    }
    void CreateFlock()
    {
        flocksTransform = new Transform[flockNum];
        flockPos = new Vector3[flockNum];
        velFlocks = new Vector3[flockNum];
        curentFlock = new int[fishNum];

        for (int f = 0; f < flockNum; f++)
        {
            GameObject nobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nobj.SetActive(debug);
            flocksTransform[f] = nobj.transform;
            Vector3 rdvf = Random.onUnitSphere * fragmentedFlock;
            flocksTransform[f].position = thisTransform.position;
            flockPos[f] = new Vector3(rdvf.x, Mathf.Abs(rdvf.y * fragmentedFlockYLimit), rdvf.z);
            flocksTransform[f].parent = thisTransform;
        }
    }

    private Vector3[] Flock()
    {
        // Implement flocking logic here to get neighboring boid positions
        // Return an array of Vector3 positions
        return new Vector3[0];
    }

    private Vector3 Cohere(Vector3[] boidLocations)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (var boidLocation in boidLocations)
        {
            float distance = Vector3.Distance(location, boidLocation);

            if (distance > 0 && distance < 50f) // Change 50f to your desired neighborhood radius
            {
                sum += boidLocation;
                count++;
            }
        }

        if (count > 0)
        {
            Vector3 averagePosition = sum / count;
            return SteerTo(averagePosition);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 SteerTo(Vector3 target)
    {
        Vector3 desired = target - location;
        float distance = desired.magnitude;

        if (distance > 0)
        {
            desired.Normalize();
            desired *= MAX_SPEED;

            Vector3 steeringForce = desired - velocity;
            return Vector3.ClampMagnitude(steeringForce, MAX_FORCE);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void WrapIfNeeded()
    {
        // Implement wrapping logic if needed
    }

    private void RenderBoid()
    {
        // Implement rendering logic here to draw the boid
    }
}
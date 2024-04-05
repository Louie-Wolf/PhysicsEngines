using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MovingCubeController : MonoBehaviour
{
    private Rigidbody thisCube;
    private Rigidbody otherCube;
    private String nameOfOtherCube = "StationaryCube";
    private float thisCubeMass;
    private float otherCubeMass;

    //Is the wind still blowing?
    private bool blowing = true;

    //The absolute speed of the wind blowing
    public float windSpeed = 8f; // m/s

    //The direction in which the wind is blowing (should have length = 1)
    private Vector3 windDirection = new Vector3(1f, 0f, 0f);

    //The length of one side of the cube
    private float sideLength = 1; // m

    //The density of the air
    public float airDensity = 1.229f; // kg/m**3

    //This number represents how efficiently the wind transfers its impulse to the cube.
    public float dragCoefficient = 1.2f;

    //The length of the spring used to simulate the collision.
    public float springLength = 1f; // m

    //The amount of force needed to compress the spring by a certain distance.
    public float springConstant = 165f; // N/m

    private float currentTimeStep; // s
    private List<List<float>> timeSeries = new List<List<float>>();



    // Start is called before the first frame update
    void Start()
    {
        thisCube = GetComponent<Rigidbody>();
        otherCube = GameObject.Find(nameOfOtherCube).GetComponent<Rigidbody>();
        thisCubeMass = thisCube.mass;
        otherCubeMass = otherCube.mass;
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        // While the wind is blowing, the windforce pushes the cube
        if (blowing)
        {
            AddWindForce();
        }

        // When the cubes come closer to each other, then the length of the spring, then the spring gets compressed and pushes both cubes apart.
        float distanceBetweenCubes = Mathf.Abs((thisCube.position.x + 0.5f * sideLength) - (otherCube.position.x - 0.5f * sideLength));
        float springEnergy = 0; // J
        if (distanceBetweenCubes < springLength)
        {
            // The wind stops blowing when the spring starts getting compressed
            blowing = false;

            float compressDist = MathF.Abs(springLength - distanceBetweenCubes); // m
            AddSpringForce(compressDist);
            springEnergy = 0.5f * springConstant * Mathf.Pow(compressDist, 2);
        }

        // Adds the data to the list, so it can be written into the csv file later
        currentTimeStep += Time.deltaTime;
        float impulse = thisCube.mass * thisCube.velocity.x; // kg * m/s
        float kinEnergy = 0.5f * thisCube.mass * Mathf.Pow(thisCube.velocity.x, 2); // J
        timeSeries.Add(new List<float>() { currentTimeStep, thisCube.position.x, thisCube.velocity.x, impulse, kinEnergy, springEnergy });
    }

    //Called when a collision is registered
    void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.name == nameOfOtherCube)
        {
            Debug.Log("The spring force is to small. The cubes have collided");
        }
    }

    void OnApplicationQuit()
    {
        WriteTimeSeriesToCSV();
    }

    private void AddWindForce() 
    {
        float relativeWindSpeed = (windSpeed * windDirection - thisCube.velocity).magnitude; // m/s
        float windForce = 0.5f * airDensity * Mathf.Pow(sideLength, 2) * Mathf.Pow(relativeWindSpeed, 2) * dragCoefficient; // N
        thisCube.AddForce(windDirection * windForce);
    }

    private void AddSpringForce(float compressDist)
    {
        float springForce = -(compressDist * springConstant); // N

        thisCube.AddForce(windDirection * springForce);
        otherCube.AddForce((-windDirection) * springForce);
    }

    private void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("movingCube_stats.csv"))
        {
            streamWriter.WriteLine("t,x(t),v(t),p(t),eKin(t),eSpr(t)");

            foreach (List<float> timeStep in timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
}

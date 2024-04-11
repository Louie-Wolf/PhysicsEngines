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

    //The number of swings the cube does in the beginning
    public int numberOfSwings = 3;

    //Counts the number of swings that this cube has done
    private double swingCounter = 0;

    //The velocity measured in the last calculation step
    private float previousVelocity = 0f;

    //Is the wind still blowing?
    private bool blowing = false;

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
    public float collisionSpringLength = 1f; // m

    //The amount of force needed to compress the oscillation spring by a certain distance.
    public float oscillationSpringConstant = 40f; // N/m

    //The amount of force needed to compress the collision spring by a certain distance.
    private float collisionSpringConstant = 0f; // N/m

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
        // Do the harmonic oscillation for the desired number of swings and then start the wind
        if (swingCounter < numberOfSwings)
        {
            DoHarmonicOscillation();
            // If the cube changes direction, the swing counter is incremented by half a swing
            if (previousVelocity * thisCube.velocity.x < 0)
            {
                swingCounter += 0.5;
            }

            // Start the wind when the desired number of swings has been reached
            if (swingCounter == numberOfSwings)
            {
                blowing = true;
            }
        }

        // While the wind is blowing, the wind-force pushes the cube
        if (blowing)
        {
            AddWindForce();

            // Update the necessary collision spring constant, assuming the current velocity is the final velocity
            collisionSpringConstant = calcCollisionSpring();
        }

        // When the cubes come closer to each other, then the length of the spring, the wind stops and the collision-spring gets compressed and pushes both cubes apart.
        float distanceBetweenCubes = Mathf.Abs((thisCube.position.x + 0.5f * sideLength) - (otherCube.position.x - 0.5f * sideLength));
        float springEnergy = 0; // J
        if (distanceBetweenCubes < collisionSpringLength)
        {
            // The wind stops blowing when the spring starts getting compressed
            blowing = false;

            float compressDist = MathF.Abs(collisionSpringLength - distanceBetweenCubes); // m
            AddSpringForce(compressDist);
            springEnergy = 0.5f * collisionSpringConstant * Mathf.Pow(compressDist, 2);
        }

        // Adds the data to the list, so it can be written into the csv file later
        currentTimeStep += Time.deltaTime;
        float impulse = thisCube.mass * thisCube.velocity.x; // kg * m/s
        float kinEnergy = 0.5f * thisCube.mass * Mathf.Pow(thisCube.velocity.x, 2); // J
        timeSeries.Add(new List<float>() { currentTimeStep, thisCube.position.x, thisCube.velocity.x, impulse, kinEnergy, springEnergy });

        previousVelocity = thisCube.velocity.x;
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



    // Oscillates the cube around x = 0
    private void DoHarmonicOscillation()
    {
        // Calculate spring force on body for x component of force vector
        float forceX = -thisCube.position.x * oscillationSpringConstant;
        thisCube.AddForce(new Vector3(forceX, 0f, 0f));
    }

    private void AddWindForce() 
    {
        float relativeWindSpeed = (windSpeed * windDirection - thisCube.velocity).magnitude; // m/s
        float windForce = 0.5f * airDensity * Mathf.Pow(sideLength, 2) * Mathf.Pow(relativeWindSpeed, 2) * dragCoefficient; // N
        thisCube.AddForce(windDirection * windForce);
    }

    private void AddSpringForce(float compressDist)
    {
        float springForce = -(compressDist * collisionSpringConstant); // N

        thisCube.AddForce(windDirection * springForce);
        otherCube.AddForce((-windDirection) * springForce);
    }

    private void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("Documents/Teil2/movingCube_stats.csv"))
        {
            streamWriter.WriteLine("t,x(t),v(t),p(t),eKin(t),eSpr(t)");

            foreach (List<float> timeStep in timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }

    private float calcCollisionSpring()
    {
        //Current kinetic energy of this cube
        float eKin = 0.5f * thisCubeMass * Mathf.Pow(thisCube.velocity.x, 2); // J

        //Calculate the spring constant needed to absorb all of the kinetic energy
        float requiredSpringConstant = 2f * eKin / Mathf.Pow(collisionSpringLength, 2); // N/m

        // +1% to make sure the cubes don't touch
        return requiredSpringConstant * 1.01f;
    }
}

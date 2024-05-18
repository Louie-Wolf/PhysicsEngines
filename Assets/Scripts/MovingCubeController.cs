using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MovingCubeController : MonoBehaviour
{
    //The density of the air
    [SerializeField]
    float airDensity = 1.229f; // kg/m**3

    //This number represents how efficiently the wind transfers its impulse to the cube.
    [SerializeField]
    float dragCoefficient = 1.2f;

    //The length of the spring used to simulate the collision.
    [SerializeField]
    float collisionSpringLength = 1f; // m

    //The amount of force needed to compress the oscillation spring by a certain distance.
    [SerializeField]
    float oscillationSpringConstant = 40f; // N/m

    //The number of swings the cube does in the beginning
    [SerializeField]
    float numberOfSwings = 3.0f;

    //The absolute speed of the wind blowing
    [SerializeField]
    float windSpeed = 8f; // m/s

    //Counts the amount of swings that this cube has done
    float swingCounter = 0.0f;

    //The velocity measured in the last calculation step
    float previousVelocity = 0f;

    //Is the wind still blowing?
    bool blowing = false;

    //The direction in which the wind is blowing (should have length = 1)
    Vector3 windDirection = new (1f, 0f, 0f);

    //The length of one side of the cube
    readonly float sideLength = 1; // m

    //The amount of force needed to compress the collision spring by a certain distance.
    float collisionSpringConstant = 0f; // N/m

    //Mass of this cube
    float thisCubeMass;

    float currentTimeStep; // s
    readonly List<List<float>> timeSeries = new();
    Rigidbody thisCube;
    Rigidbody otherCube;
    readonly string nameOfOtherCube = "StationaryCube";

    float velocityRedCubeBefore = 0f;
    float currentSpringForce = 0f;

    void Start()
    {
        thisCube = GetComponent<Rigidbody>();
        otherCube = GameObject.Find(nameOfOtherCube).GetComponent<Rigidbody>();
        thisCubeMass = thisCube.mass;
    }

    void FixedUpdate()
    {
        // Do the harmonic oscillation for the desired number of swings and then start the wind
        if (swingCounter < numberOfSwings)
        {
            currentSpringForce = DoHarmonicOscillation();
            // If the cube changes direction, the swing counter is incremented by half a swing
            if (previousVelocity * thisCube.velocity.x < 0)
            {
                swingCounter += 0.5f;
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
            collisionSpringConstant = CalcCollisionSpring();
        }

        // As the cubes come closer together than the length of the spring, the wind stops and the collision spring compresses, pushing the two cubes apart.
        float distanceBetweenCubes = Mathf.Abs((thisCube.position.x + 0.5f * sideLength) - (otherCube.position.x - 0.5f * sideLength));
        float springEnergy = 0; // J
        if (distanceBetweenCubes < collisionSpringLength)
        {
            // The wind stops blowing when the spring starts getting compressed
            blowing = false;

            float compressDist = Mathf.Abs(collisionSpringLength - distanceBetweenCubes); // m
            AddSpringForce(compressDist);
            springEnergy = 0.5f * collisionSpringConstant * Mathf.Pow(compressDist, 2);
        }

        // Adds the data to the list, so it can be written into the csv file later
        currentTimeStep += Time.fixedDeltaTime;
        float accelerationRedCube =  (thisCube.velocity.x - velocityRedCubeBefore) / Time.fixedDeltaTime;
        velocityRedCubeBefore = thisCube.velocity.x;
        float impulse = thisCube.mass * thisCube.velocity.x; // kg * m/s
        float kinEnergy = 0.5f * thisCube.mass * Mathf.Pow(thisCube.velocity.x, 2); // J
        timeSeries.Add(new List<float>() { currentTimeStep, thisCube.position.x, thisCube.velocity.x, impulse, kinEnergy, springEnergy, accelerationRedCube, currentSpringForce });

        previousVelocity = thisCube.velocity.x;
    }

    //Called when a collision is registered
    void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.name == nameOfOtherCube)
        {
            Debug.LogWarning("The spring force is to small. The cubes have collided!");
        }
    }

    void OnApplicationQuit()
    {
        WriteTimeSeriesToCSV();
    }

    // Oscillates the cube around x = 0
    private float DoHarmonicOscillation()
    {
        float strainDistance = thisCube.position.x; // because the cube oscillates around x=0
        float springForce = -(strainDistance * oscillationSpringConstant);   // N
        thisCube.AddForce(new Vector3(springForce, 0f, 0f));
        return springForce;
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

        thisCube.AddForce(new Vector3(springForce, 0, 0));
        otherCube.AddForce(new Vector3(-springForce, 0, 0));
    }

    private void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("Documents/Teil3/movingCube_stats.csv"))
        {
            streamWriter.WriteLine("t,x(t),v(t),p(t),eKin(t),eSpr(t), a(t), F_Feder(t)");

            foreach (List<float> timeStep in timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }

    private float CalcCollisionSpring()
    {
        //Current kinetic energy of this cube
        float eKin = 0.5f * thisCubeMass * Mathf.Pow(thisCube.velocity.x, 2); // J

        //Calculate the spring constant needed to absorb all of the kinetic energy
        float requiredSpringConstant = 2f * eKin / Mathf.Pow(collisionSpringLength, 2); // N/m

        // +1% to make sure the cubes don't touch
        return requiredSpringConstant * 1.01f;
    }
}

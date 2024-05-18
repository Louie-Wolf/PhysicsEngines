using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StationaryCubeController : MonoBehaviour
{
    Rigidbody thisCube;
    Rigidbody LCube1;
    Rigidbody LCube2;
    Rigidbody LCube3;

    const string nameOfGround = "Plane";

    // this field is public because it is used in the camera controller
    [HideInInspector]
    public bool isJoined = false;

    float massCombBody;

    // Values describing translation of the combined body:
    [HideInInspector]
    public Vector3 centerOfMassCombBody = new Vector3(25f, 0f, 0.5f); // m
    Vector3 previousCenterMassCombBody = Vector3.zero;   // m
    Vector3 velocityCombBody = Vector3.zero; // m / s
    float impulseCombBody = 0; // kg * m / s
    float kineticEnergyCombBody = 0;    // J

    // Values describing the rotation of the combined body:
    float angle = 0f; // rad
    float previousAngle = 0f; // rad
    float angularVelocity = 0;  // rad / s
    float momentOfInertia = 0;  // kg * m^2
    float intrinsicAngularMomentum = 0;  // kg * m^2 / s
    float orbitalAngularMomentum = 0;   // kg * m^2 / s
    float totalAngularMomentum = 0; // kg * m^2 / s
    float rotEnergy = 0;    // J

    private bool firstTime = true;


    float currentTimeStep; // s
    readonly List<List<float>> timeSeries = new();

    // Start is called before the first frame update
    void Start()
    {
        thisCube = GameObject.Find("StationaryCube").GetComponent<Rigidbody>();
        LCube1 = GameObject.Find("LCube1").GetComponent<Rigidbody>();
        LCube2 = GameObject.Find("LCube2").GetComponent<Rigidbody>();
        LCube3 = GameObject.Find("LCube3").GetComponent<Rigidbody>();

        // Mass of the combined body after the inelastic collision
        massCombBody = thisCube.mass + LCube1.mass + LCube2.mass + LCube3.mass;   // kg
    }

    void FixedUpdate()
    {
        // Absolut position of the cubes
        Vector3 posThisCube = thisCube.position; // m
        Vector3 posLCube1 = LCube1.position; // m
        Vector3 posLCube2 = LCube2.position; // m
        Vector3 posLCube3 = LCube3.position; // m

        if (isJoined)
        {

            // The center of mass of a combined body is the mean of the positional vectors of its parts weighted by mass:
            centerOfMassCombBody = (posThisCube + posLCube1 + posLCube2 + posLCube3) / 4f;

            // Position of the cubes relative to the centerOfMass of the combined body
            Vector3 posRelThisCube = posThisCube - centerOfMassCombBody; // m
            Vector3 posRelLCube1 = posLCube1 - centerOfMassCombBody; // m
            Vector3 posRelLCube2 = posLCube2 - centerOfMassCombBody; // m
            Vector3 posRelLCube3 = posLCube3 - centerOfMassCombBody; // m

            // The angle of the rotation is determined by looking at the vector pointing from the center of Mass to one of the yellow cubes
            angle = CalculateRotationAngle(posRelLCube1); // rad

            // The first time the cubes are joined, the previous values need to be set
            if (firstTime)
            {
                previousAngle = angle;
                previousCenterMassCombBody = centerOfMassCombBody;
                firstTime = false;
            }

            // The derivative of the position of the center of mass is the velocity of the combined body:
            velocityCombBody = (centerOfMassCombBody - previousCenterMassCombBody) / Time.fixedDeltaTime;  // m / s
            impulseCombBody = velocityCombBody.x * massCombBody;    // kg * m / s
            kineticEnergyCombBody = 0.5f * massCombBody * Mathf.Pow(velocityCombBody.x, 2f); // J

            float deltaAngle = angle - previousAngle; // rad
            // When the angle starts at 0 again, two PI have to be added to the delta, so it's not negative
            if (deltaAngle < 0f)
            {
                deltaAngle += 2f * Mathf.PI; // rad
            }
            angularVelocity = deltaAngle / Time.fixedDeltaTime; // rad / s

            // Massenträgheitsmoment J
            momentOfInertia = thisCube.mass * Mathf.Pow(posRelThisCube.magnitude, 2f)
                              + LCube1.mass * Mathf.Pow(posRelLCube1.magnitude, 2f)
                              + LCube2.mass * Mathf.Pow(posRelLCube2.magnitude, 2f)
                              + LCube3.mass * Mathf.Pow(posRelLCube3.magnitude, 2f)
                              + 4f/6f * thisCube.mass;     // kg * m^2

            // Eigen-Drehimpuls
            intrinsicAngularMomentum = momentOfInertia * angularVelocity;     // kg * m^2 / s

            // Bahn-Drehimpuls
            orbitalAngularMomentum = - Vector3.Cross(centerOfMassCombBody, velocityCombBody).magnitude * massCombBody;    // kg * m^2 / s

            // Drehimpuls L
            totalAngularMomentum = intrinsicAngularMomentum + orbitalAngularMomentum;    // kg * m^2 / s

            // Rotationsenergie T
            rotEnergy = 0.5f * momentOfInertia * Mathf.Pow(angularVelocity, 2f); // J
        }

        // Adds the data to the list, so it can be written into the csv file later
        currentTimeStep += Time.fixedDeltaTime;
        float impulse = thisCube.mass * thisCube.velocity.x; // kg * m/s
        float kinEnergy = 0.5f * thisCube.mass * Mathf.Pow(thisCube.velocity.x, 2f); // J
        timeSeries.Add(new List<float>() {
            currentTimeStep,
            thisCube.position.x, thisCube.position.z, thisCube.velocity.x, impulse, kinEnergy,
            angle, angularVelocity, intrinsicAngularMomentum, orbitalAngularMomentum, totalAngularMomentum, rotEnergy,
            centerOfMassCombBody.x, centerOfMassCombBody.z, velocityCombBody.x, impulseCombBody, kineticEnergyCombBody
        });

        // update the values
        previousAngle = angle;
        previousCenterMassCombBody = centerOfMassCombBody;
    }

    //Called when a collision is registered
    void OnCollisionEnter(Collision collision)
    {
        if (isJoined)
        {
            // We only want to create a new joint the first time the cubes collide
            return;
        }
        GameObject target = collision.gameObject;

        // if the blue cube touches something other than the ground, a fixed joint is added connecting the two bodies
        if (target.name != nameOfGround)
        {
            isJoined = true;
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = target.GetComponent<Rigidbody>();
        }

    }

    void OnApplicationQuit()
    {
        WriteTimeSeriesToCSV();
    }

    private float CalculateRotationAngle(Vector3 radius) 
    {
        // returns the angle between the radius-vector and the z-Axis
        return Mathf.Atan2(-radius.x, radius.z) + Mathf.PI;
    }

    private void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("Documents/Teil3/stationaryCube_stats.csv"))
        {
            streamWriter.WriteLine("t,x(t),z(t),v(t),p(t),eKin(t),alpha(t),vRot(t)_L,pIntRot(t)_L,pOrbRot(t)_L,pTotRot(t)_L,eRot(t)_L,xTrans(t),zTrans(t),vTrans(t),pTrans(t),eTrans(t)");

            foreach (List<float> timeStep in timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
}

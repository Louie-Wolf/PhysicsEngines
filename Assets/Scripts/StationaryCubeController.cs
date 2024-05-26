using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StationaryCubeController : MonoBehaviour
{
    Rigidbody thisCube;
    Rigidbody LCube1;
    Rigidbody LCube2;
    Rigidbody LCube3;

    float massCombBody; //kg

    // values describing translation of the combined body:
    [HideInInspector]
    public Vector3 centerOfMassCombBody = new Vector3(25f, 0f, 0.5f); // m
    Vector3 previousCenterMassCombBody = Vector3.zero;   // m
    Vector3 velocityCombBody = Vector3.zero; // m / s
    float combinedBodyMomentum = 0; // kg * m / s
    float kineticEnergyCombBody = 0;    // J

    // values describing the rotation of the combined body:
    float angle = 0f; // rad
    float previousAngle = 0f; // rad
    float angularVelocity = 0f;  // rad / s
    float momentOfInertia = 0f;  // kg * m^2
    float intrinsicAngularMomentum = 0f;  // kg * m^2 / s
    float orbitalAngularMomentum = 0f;   // kg * m^2 / s
    float totalAngularMomentum = 0f; // kg * m^2 / s
    float rotEnergy = 0f;    // J

    bool isFirstCalculationStep = true;
    float currentTimeStep; // s
    readonly List<List<float>> timeSeries = new();

    const string nameOfGround = "Plane";

    // reference used in the camera controller
    [HideInInspector]
    public bool isJoined = false;

    // Start is called before the first frame update
    void Start()
    {
        thisCube = GameObject.Find("StationaryCube").GetComponent<Rigidbody>();
        LCube1 = GameObject.Find("LCube1").GetComponent<Rigidbody>();
        LCube2 = GameObject.Find("LCube2").GetComponent<Rigidbody>();
        LCube3 = GameObject.Find("LCube3").GetComponent<Rigidbody>();

        // mass of the combined body after the inelastic collision
        massCombBody = thisCube.mass + LCube1.mass + LCube2.mass + LCube3.mass;   // kg
    }

    void FixedUpdate()
    {
        // absolute position of the cubes
        Vector3 posThisCube = thisCube.position; // m
        Vector3 posLCube1 = LCube1.position; // m
        Vector3 posLCube2 = LCube2.position; // m
        Vector3 posLCube3 = LCube3.position; // m

        if (isJoined)
        {

            // the center of mass of a combined body is the mean of the positional vectors of its parts weighted by mass:
            centerOfMassCombBody = (posThisCube + posLCube1 + posLCube2 + posLCube3) / 4f;

            // position of the cubes relative to the centerOfMass of the combined body
            Vector3 posRelThisCube = posThisCube - centerOfMassCombBody; // m
            Vector3 posRelLCube1 = posLCube1 - centerOfMassCombBody; // m
            Vector3 posRelLCube2 = posLCube2 - centerOfMassCombBody; // m
            Vector3 posRelLCube3 = posLCube3 - centerOfMassCombBody; // m

            // the angle of the rotation is determined by looking at the vector pointing from the center of mass to one of the yellow cubes
            angle = CalculateRotationAngle(posRelLCube1); // rad

            // the first time the cubes are joined, the previous values need to be set
            if (isFirstCalculationStep)
            {
                previousAngle = angle;
                previousCenterMassCombBody = centerOfMassCombBody;
                isFirstCalculationStep = false;
            }

            // The derivative of the position of the center of mass is the velocity of the combined body:
            velocityCombBody = (centerOfMassCombBody - previousCenterMassCombBody) / Time.fixedDeltaTime;  // m / s
            combinedBodyMomentum = velocityCombBody.x * massCombBody;    // kg * m / s
            kineticEnergyCombBody = 0.5f * massCombBody * Mathf.Pow(velocityCombBody.x, 2f); // J

            float deltaAngle = angle - previousAngle; // rad
            // When the angle starts at 0 again, two PI have to be added to the delta, so it's not negative
            if (deltaAngle < 0f)
            {
                deltaAngle += 2f * Mathf.PI; // rad
            }
            angularVelocity = deltaAngle / Time.fixedDeltaTime; // rad / s

            // moment of inertia J
            momentOfInertia = (1f / 6f * thisCube.mass) + (thisCube.mass * Mathf.Pow(posRelThisCube.magnitude, 2f))
                              + (1f / 6f * LCube1.mass) + (LCube1.mass * Mathf.Pow(posRelLCube1.magnitude, 2f))
                              + (1f / 6f * LCube2.mass) +( LCube2.mass * Mathf.Pow(posRelLCube2.magnitude, 2f))
                              + (1f / 6f * LCube3.mass) + (LCube3.mass * Mathf.Pow(posRelLCube3.magnitude, 2f)); // kg * m^2

            // intrinsic angular momentum L_intri
            intrinsicAngularMomentum = momentOfInertia * angularVelocity;     // kg * m^2 / s

            // orbital angular momentum L_orbit
            orbitalAngularMomentum = - Vector3.Cross(centerOfMassCombBody, velocityCombBody * massCombBody).magnitude;    // kg * m^2 / s

            // total angular momentum L
            totalAngularMomentum = intrinsicAngularMomentum + orbitalAngularMomentum;    // kg * m^2 / s

            // rotational energy T
            rotEnergy = 0.5f * momentOfInertia * Mathf.Pow(angularVelocity, 2f); // J
        }

        // adds the data to the list, so it can be written into the csv file later
        currentTimeStep += Time.fixedDeltaTime;
        float momentum = thisCube.mass * thisCube.velocity.x; // kg * m/s
        float kinEnergy = 0.5f * thisCube.mass * Mathf.Pow(thisCube.velocity.x, 2f); // J

        timeSeries.Add(new List<float>() {
            currentTimeStep,
            thisCube.position.x, thisCube.position.z, thisCube.velocity.x, momentum, kinEnergy,
            angle, angularVelocity, intrinsicAngularMomentum, orbitalAngularMomentum, totalAngularMomentum, rotEnergy,
            centerOfMassCombBody.x, centerOfMassCombBody.z, velocityCombBody.x, combinedBodyMomentum, kineticEnergyCombBody
        });

        // update values
        previousAngle = angle;
        previousCenterMassCombBody = centerOfMassCombBody;
    }

    //Called when a collision is registered
    void OnCollisionEnter(Collision collision)
    {
        if (isJoined) // only create a new joint the first time the cubes collide
        {
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

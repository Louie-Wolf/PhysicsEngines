using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;

/*
    Accelerates the cube to which it is attached, modelling an harmonic oscillator.
    Writes the position, velocity and acceleration of the cube to a CSV file.
    
    Remark: For use in "Physics Engines" module at ZHAW, part of physics lab
    Author: kemf
    Version: 1.0
*/
public class CubeController : MonoBehaviour
{
    private Rigidbody rigidBody;

    [SerializeField]
    private int springConstant; // N/m

    private float currentTimeStep; // s

    [SerializeField]
    private float maxTime;
    
    private List<List<float>> timeSeries;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        timeSeries = new List<List<float>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate() {
        //float forceX; // N

        // Calculate spring force on body for x component of force vector
        //forceX = -rigidBody.position.x * collisionSpringConstant;
        //rigidBody.AddForce(new Vector3(forceX, 0f, 0f));

        currentTimeStep += Time.deltaTime;
        //if (currentTimeStep <= maxTime) {
        //    timeSeries.Add(new List<float>() { currentTimeStep, rigidBody.position.x, rigidBody.velocity.x, forceX });
        //}else {
        //    print("done!");
        //}


        float forceZ; //N
        // Calculate spring force on body for z component of force vector
        forceZ = -rigidBody.position.z * springConstant;
        rigidBody.AddForce(new Vector3(0f, 0f, forceZ));

        if (currentTimeStep <= maxTime) {
            timeSeries.Add(new List<float>() { currentTimeStep, rigidBody.position.z, rigidBody.velocity.z, forceZ });
        } else {
            print("done!");
        }
    }

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
    }

    void WriteTimeSeriesToCSV() {
        using (var streamWriter = new StreamWriter("time_series.csv")) {
            streamWriter.WriteLine("t,x(t),v(t),F(t) (added)");
            
            foreach (List<float> timeStep in timeSeries) {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
}

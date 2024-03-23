using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MovingCubeController : MonoBehaviour
{
    private Rigidbody rigidBody;

    //Is the wind still blowing?
    private bool blowing = true;

    //The absolute speed of the wind blowing
    private float windSpeed = 8f; // m/s

    //The direction in which the wind is blowing (should have length = 1)
    private Vector3 windDirection = new Vector3(1f, 0f, 0f);

    //The area of one side of the cube, where the wind blows onto
    private float sideArea = 1; // m**2

    //The density of the air
    private float airDensity = 1.229f; // kg/m**3

    //This number represents how efficiently the wind transfers its impulse to the cube
    //(0 --> all the air slips past the cube | 1 --> the air transfers it impulse perfectly)
    private float windResistance = 0.5f;

    private float currentTimeStep; // s
    private List<List<float>> timeSeries = new List<List<float>>();



    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        if (blowing)
        {
            float relativeWindSpeed = (windSpeed * windDirection - rigidBody.velocity).magnitude;
            float windForce = airDensity * sideArea * Mathf.Pow(relativeWindSpeed,2) * windResistance;
            rigidBody.AddForce(windDirection * windForce);
        }

        currentTimeStep += Time.deltaTime;
        timeSeries.Add(new List<float>() { currentTimeStep, rigidBody.position.x, rigidBody.velocity.x });
    }

    //Called when a collision is registered
    void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.name == "StationaryCube")
        {
            blowing = false;
        }
    }

    void OnApplicationQuit()
    {
        WriteTimeSeriesToCSV();
    }

    void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("movingCube_stats.csv"))
        {
            streamWriter.WriteLine("t,x(t),v(t)");

            foreach (List<float> timeStep in timeSeries)
            {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
}

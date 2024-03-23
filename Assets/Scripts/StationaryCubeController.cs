using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StationaryCubeController : MonoBehaviour
{
    private Rigidbody rigidBody;

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
        currentTimeStep += Time.deltaTime;
        timeSeries.Add(new List<float>() { currentTimeStep, rigidBody.position.x, rigidBody.velocity.x });
    }

    void OnApplicationQuit()
    {
        WriteTimeSeriesToCSV();
    }

    void WriteTimeSeriesToCSV()
    {
        using (var streamWriter = new StreamWriter("stationaryCube_stats.csv"))
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

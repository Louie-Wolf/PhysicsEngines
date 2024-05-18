using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    Rigidbody camera;
    private Rigidbody blueCube;
    StationaryCubeController blueCubeScript;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Rigidbody>();
        blueCube = GameObject.Find("StationaryCube").GetComponent<Rigidbody>();
        blueCubeScript = GameObject.Find("StationaryCube").GetComponent<StationaryCubeController>();
    }

    void FixedUpdate()
    {
        float newXCord;
        if (blueCubeScript.isJoined)
        {
            newXCord = blueCubeScript.centerOfMassCombBody.x - 1;
            camera.MovePosition(new Vector3(newXCord, camera.position.y, camera.position.z));
        }
        else
        {
            newXCord = blueCube.position.x;
        }
        camera.MovePosition(new Vector3(newXCord, camera.position.y, camera.position.z));
    }
}

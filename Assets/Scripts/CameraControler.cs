using UnityEngine;

public class CameraControler : MonoBehaviour
{
    Transform blueCubeTF;
    StationaryCubeController blueCubeScript;

    void Start()
    {
        GameObject blueCube = GameObject.Find("StationaryCube");
        blueCubeTF = blueCube.transform;
        blueCubeScript = blueCube.GetComponent<StationaryCubeController>();
    }

    void FixedUpdate()
    {
        float newXCord;
        if (blueCubeScript.isJoined)
        {
            newXCord = blueCubeScript.centerOfMassCombBody.x - 1f;
        }
        else
        {
            newXCord = blueCubeTF.position.x;
        }
        transform.Translate(new Vector3(newXCord - transform.position.x, 0f, 0f));
    }
}

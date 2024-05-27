using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private float time = 0f;
    [SerializeField]
    private float maxTime = 41f;
    private bool hasFinished = false;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (!hasFinished && time > maxTime) {
            hasFinished = true;
            Debug.Log("Finished!");
        }
    }
}

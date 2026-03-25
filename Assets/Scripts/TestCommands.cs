using UnityEngine;

public class TestCommands : MonoBehaviour
{
        // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 1;
            Debug.Log("Time scale set to Normal");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 1.5f;
            Debug.Log("Time scale set to 1.5x");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Time.timeScale = 2f;
            Debug.Log("Time scale set to 2x");
        }
    }
}

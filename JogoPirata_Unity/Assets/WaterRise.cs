using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterRise : MonoBehaviour
{
    public float speed = 0.5f;
    public Transform gameOverLine;

    void Update()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);

        if (transform.position.y >= gameOverLine.position.y)
        {
            Debug.Log("💀 Afundou!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
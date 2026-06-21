using UnityEngine;
using UnityEngine.SceneManagement;

public class FloodSystem : MonoBehaviour
{
    public float flood = 0f;
    public float maxFlood = 100f;

    void Update()
    {
        // teste simples: água sobe sozinha
        flood += Time.deltaTime * 2f;

        // quando chega em 100%
        if (flood >= maxFlood)
        {
            Debug.Log("💀 O navio afundou!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // isso aqui é pros buracos aumentarem a água depois
    public void AddFlood(float value)
    {
        flood += value;
    }
}
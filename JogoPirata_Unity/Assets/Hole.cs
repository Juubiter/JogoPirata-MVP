using UnityEngine;

public class Hole : MonoBehaviour
{
    public float damage = 20f;

    void Start()
    {
        FloodSystem floodSystem = FindObjectOfType<FloodSystem>();
        floodSystem.AddFlood(damage);
    }
}

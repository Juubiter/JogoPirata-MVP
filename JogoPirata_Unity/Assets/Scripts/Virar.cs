using UnityEngine;

public class Virar : MonoBehaviour
{
    public float tempo = 10f;
    void VirarLado()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    void Start()
    {
        InvokeRepeating(nameof(VirarLado), tempo, tempo);
    }
}

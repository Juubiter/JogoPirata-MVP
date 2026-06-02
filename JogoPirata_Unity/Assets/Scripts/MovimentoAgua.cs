using UnityEngine;

public class MovimentoAgua : MonoBehaviour
{
    public float velocidade = 2f;
    private float largura;

    void Start()
    {
        largura = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.Translate(Vector3.left * velocidade * Time.deltaTime);
        if (transform.position.x <= -largura)
        {
            transform.position = new Vector3(transform.position.x + (largura * 2f), transform.position.y, transform.position.z);
        }
    }
}
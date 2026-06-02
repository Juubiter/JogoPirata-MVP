using UnityEngine;

public class MovimentoObjetosFundo : MonoBehaviour
{
    public float velocidade = 0.5f;
    public float limiteEsquerdo = -12f; 
    public float pontoDeRetorno = 12f;

    void Update()
    {
        transform.Translate(Vector3.left * velocidade * Time.deltaTime);
        if (transform.position.x <= limiteEsquerdo)
        {
            transform.position = new Vector3(pontoDeRetorno, transform.position.y, transform.position.z);
        }
    }
}
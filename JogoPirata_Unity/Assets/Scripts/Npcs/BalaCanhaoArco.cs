using UnityEngine;

public class BalaCanhaoArco : MonoBehaviour
{
    public float duracaoVoo = 1.2f;
    public float alturaArco = 3f;

    private Vector3 inicio;
    private Vector3 destino;
    private float tempo;
    private bool voando = false;

    private Transform alvo;

    public void DefinirAlvo(Transform novoAlvo)
    {
        alvo = novoAlvo;
    }

    public void Disparar(Vector3 pontoInicial, Vector3 pontoDestino)
    {
        inicio = pontoInicial;
        destino = pontoDestino;
        tempo = 0f;
        voando = true;

        transform.position = inicio;
    }

    void Update()
    {
        if (!voando)
            return;

        tempo += Time.deltaTime;
        float t = tempo / duracaoVoo;

        if (t >= 1f)
        {
            transform.position = destino;
            voando = false;

            if (alvo != null)
            {
                VidaNavioInimigo vida = alvo.GetComponentInParent<VidaNavioInimigo>();

                if (vida != null)
                    vida.ReceberDano();
            }

            Destroy(gameObject);
            return;
        }

        Vector3 pos = Vector3.Lerp(inicio, destino, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * alturaArco;

        transform.position = pos;
    }
}
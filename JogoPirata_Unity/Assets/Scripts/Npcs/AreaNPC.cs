using UnityEngine;

public class AreaNPC : MonoBehaviour
{
    public enum TipoArea
    {
        Conves,
        Meio,
        Baixo,
        EscadaEsquerda,
        EscadaDireita
    }

    public TipoArea tipo;

    public Vector2 PontoAleatorio()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        Vector2 centro = (Vector2)transform.position + box.offset;

        float x = Random.Range(
            centro.x - box.size.x / 2f,
            centro.x + box.size.x / 2f
        );

        float y = Random.Range(
            centro.y - box.size.y / 2f,
            centro.y + box.size.y / 2f
        );

        return new Vector2(x, y);
    }
}
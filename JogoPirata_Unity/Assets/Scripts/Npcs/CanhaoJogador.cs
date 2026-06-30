using UnityEngine;

public class CanhaoJogador : MonoBehaviour
{
    [Header("Configuração do Tiro")]
    public GameObject prefabBalaJogador;
    public Transform pontoDisparo;

    [Header("Visual da Bala")]
    public float tamanhoBala = 0.08f;
    public float velocidadeBala = 5f;

    public void Atirar()
{
    if (prefabBalaJogador == null || pontoDisparo == null)
    {
        Debug.LogWarning("Falta bala ou ponto de disparo!");
        return;
    }

    EnemyShip inimigo = FindFirstObjectByType<EnemyShip>();

    if (inimigo == null)
    {
        Debug.LogWarning("Nenhum navio inimigo encontrado!");
        return;
    }

    Transform alvoInimigo = inimigo.transform.Find("AlvoCentro");

    if (alvoInimigo == null)
        alvoInimigo = inimigo.transform;

    GameObject bala = Instantiate(prefabBalaJogador, pontoDisparo.position, Quaternion.identity);
    bala.transform.localScale = Vector3.one * tamanhoBala;

    SpriteRenderer[] renderers = bala.GetComponentsInChildren<SpriteRenderer>();

    foreach (SpriteRenderer sr in renderers)
    {
        sr.sortingLayerName = "Fumaça";
        sr.sortingOrder = 10000;
    }

    BalaCanhaoArco arco = bala.GetComponent<BalaCanhaoArco>();

    if (arco != null)
    {
        arco.Disparar(pontoDisparo.position, alvoInimigo.position);
    }
    else
    {
        Debug.LogWarning("A bala não tem o script BalaCanhaoArco!");
    }

    Debug.Log("BALA EM ARCO DISPARADA CONTRA: " + inimigo.name);
}
}
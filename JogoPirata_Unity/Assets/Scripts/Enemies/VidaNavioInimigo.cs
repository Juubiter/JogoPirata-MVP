using System.Collections;
using UnityEngine;

public class VidaNavioInimigo : MonoBehaviour
{
    [Header("Vida visual por caveiras")]
    public GameObject[] caveiras;

    [Header("Explosões")]
    public GameObject explosaoPrefab;
    public Transform[] pontosExplosao;
    public float intervaloEntreExplosoes = 0.3f;

    [Header("Derrota")]
    public float tempoParaDesaparecer = 3f;

    private int danoAtual = 0;
    private bool derrotado = false;

    void Start()
    {
        AtualizarCaveiras();
    }

    public void ReceberDano()
    {
        if (derrotado)
            return;

        danoAtual++;
        AtualizarCaveiras();

        Debug.Log("Navio inimigo recebeu dano: " + danoAtual + "/" + caveiras.Length);

        if (danoAtual >= caveiras.Length)
        {
            derrotado = true;
            StartCoroutine(RotinaDerrota());
        }
    }

    void AtualizarCaveiras()
    {
        for (int i = 0; i < caveiras.Length; i++)
        {
            if (caveiras[i] != null)
                caveiras[i].SetActive(i < danoAtual);
        }
    }

    IEnumerator RotinaDerrota()
    {
        Debug.Log("NAVIO INIMIGO DERROTADO!");

        // Para o navio
        EnemyShip enemy = GetComponent<EnemyShip>();

        if (enemy == null)
            enemy = GetComponentInParent<EnemyShip>();

        if (enemy != null)
            enemy.enabled = false;

        // Explosões sequenciais
        if (explosaoPrefab != null && pontosExplosao != null)
        {
            foreach (Transform ponto in pontosExplosao)
            {
                if (ponto == null)
                    continue;

                GameObject explosao = Instantiate(
                    explosaoPrefab,
                    ponto.position,
                    Quaternion.identity
                );

                // Faz a explosão ser filha do navio
                explosao.transform.SetParent(transform.root, true);

                yield return new WaitForSeconds(intervaloEntreExplosoes);
            }
        }

        // Espera antes de desaparecer
        yield return new WaitForSeconds(tempoParaDesaparecer);

        // Destrói o navio inteiro
        Destroy(transform.root.gameObject);
    }
}
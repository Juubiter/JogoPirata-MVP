using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject schoenerPrefab;   // Escuna (Fase 2)
    public GameObject warshipPrefab;    // Navio de Guerra (Fase 4)

    [Header("Pontos de spawn no fundo do cenário")]
    public Transform[] spawnPoints;

    [Header("Configurações (ajuste por fase no Inspector)")]
    public float spawnInterval = 25f;
    public int maxSimultaneous = 2;
    public bool spawnWarship = false;   // true apenas na Fase 4

    private bool isSpawning = false;

    // Chamado pelo GameManager quando a fase inicia
    public void StartSpawning()
    {
        Debug.Log("[Spawner] StartSpawning chamado!");
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) { Debug.Log("[Spawner] ERRO: Nenhum spawn point!"); return; }

        GameObject prefab = spawnWarship ? warshipPrefab : schoenerPrefab;
        if (prefab == null) { Debug.Log("[Spawner] ERRO: Prefab nulo!"); return; }

        Debug.Log("[Spawner] Spawnando navio!");
        Transform ponto = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, ponto.position, Quaternion.identity);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(5f); // pequeno delay antes do primeiro navio

        while (isSpawning)
        {
            // Conta quantos navios inimigos estão vivos na cena agora
            int ativos = FindObjectsByType<EnemyShip>().Length;

            if (ativos < maxSimultaneous)
                SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    
}
using UnityEngine;

// A Pessoa responsável pelos sistemas do navio só precisa preencher os dois métodos.
public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        Debug.Log("[ShipManager] Spawner encontrado: " + spawner);

        if (spawner == null)
        {
            Debug.LogError("[ShipManager] EnemySpawner não encontrado na cena!");
            return;
        }

        spawner.StartSpawning();
    }

    // TODO: Pessoa pelos sistemas do navio preenche aqui
    public void TriggerFire(Vector2 pos)
    {
        Debug.Log($"[ShipManager] Incêndio em {pos} — implementar!");
    }

    // TODO: Pessoa pelos sistemas do navio preenche aqui
    public void TriggerLeak(Vector2 pos)
    {
        Debug.Log($"[ShipManager] Vazamento em {pos} — implementar!");
    }
}
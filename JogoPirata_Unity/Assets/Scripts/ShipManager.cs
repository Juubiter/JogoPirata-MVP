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

    public void TriggerFire(Vector2 pos)
    {
        HUDManager hud = FindObjectOfType<HUDManager>();
        if (hud != null)
        {
            hud.TakeDamage(10f);
            Debug.Log($"[ShipManager] Incêndio em {pos} — 10 de dano causado!");
        }
    }

    public void TriggerLeak(Vector2 pos)
    {
        HUDManager hud = FindObjectOfType<HUDManager>();
        if (hud != null)
        {
            hud.TakeWaterDamage(15f);
            Debug.Log($"[ShipManager] Vazamento em {pos} — 15 de água causado!");
        }
    }
}
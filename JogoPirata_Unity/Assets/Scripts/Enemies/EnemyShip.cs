using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public float damageInterval = 8f;   // Cada X segundos causa um problema no navio
    public int maxHealth = 3;           // Quantos tiros do canhão aguenta

    // Flags para configurar o tipo de navio no Inspector:
    // escuna normal: os dois false → sorteia aleatório
    // Navio de Guerra (Fase 4): causesBoth = true
    public bool causesFloodOnly = false;
    public bool causesBoth = false;

    [Header("Pontos de dano (Transforms no cenário do navio)")]
    // Arraste aqui 2 ou 3 pontos espalhados no navio onde incêndios/vazamentos podem surgir
    public Transform[] damagePoints;

    [Header("VFX")]
    public ParticleSystem explosionParticles;

    [Header("Áudio")]
    public AudioClip explosionSound;

    private int currentHealth;
    private float damageTimer;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        damageTimer = damageInterval;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0f)
        {
            CausarDano();
            damageTimer = damageInterval;
        }
    }

    Vector2 GetRandomDamagePoint()
    {
        if (damagePoints != null && damagePoints.Length > 0)
            return damagePoints[Random.Range(0, damagePoints.Length)].position;

        return Vector2.zero; // fallback: centro do mundo
    }

    void CausarDano()
    {
        if (ShipManager.Instance == null) return;

        Vector2 pos = GetRandomDamagePoint();

        if (causesBoth)
        {
            // Fase 4: Navio de Guerra — causa os dois ao mesmo tempo
            ShipManager.Instance.TriggerFire(pos);
            ShipManager.Instance.TriggerLeak(pos);
        }
        else if (causesFloodOnly)
        {
            ShipManager.Instance.TriggerLeak(pos);
        }
        else
        {
            // Escuna normal: sorteia aleatório
            if (Random.value < 0.5f)
                ShipManager.Instance.TriggerFire(pos);
            else
                ShipManager.Instance.TriggerLeak(pos);
        }
    }

    // Chamado pelo projétil do canhão ao acertar
    public void TakeHit()
    {
        currentHealth--;
        if (currentHealth <= 0) Morrer();
    }

    void Morrer()
    {
        // Som de explosão
        if (explosionSound != null && audioSource != null)
            audioSource.PlayOneShot(explosionSound);

        // Partícula de explosão: desparentamos para não sumir junto com o objeto
        if (explosionParticles != null)
        {
            ParticleSystem ps = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1f);
        }

        Destroy(gameObject);
    }
}
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 2f;
    public Vector2 moveDirection = Vector2.left;

    [Header("Balanço")]
    public float swayAmplitude = 0.15f;
    public float swayFrequency = 1.2f;
    public float rockAngle = 5f;
    [Tooltip("Filho vazio do prefab posicionado no centro do navio. Se vazio, gira pela origem do root.")]
    public Transform rockPivot;

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
    private Vector2 basePosition;
    private Vector2 swayAxis;
    private float swayOffset;
    private Vector3 pivotOffset;   // offset world-space de root→pivô, calculado no spawn com rotação identity
    private bool hasEnteredScreen = false;

    void Start()
    {
        currentHealth = maxHealth;
        damageTimer = damageInterval;
        audioSource = GetComponent<AudioSource>();

        moveDirection = moveDirection.normalized;
        swayAxis = new Vector2(-moveDirection.y, moveDirection.x);
        swayOffset = Random.Range(0f, Mathf.PI * 2f);

        Transform root = transform.root;
        pivotOffset = null != rockPivot ? rockPivot.position - root.position : Vector3.zero;
        basePosition = (Vector2)root.position + (Vector2)pivotOffset;
    }

    void Update()
    {
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0f)
        {
            CausarDano();
            damageTimer = damageInterval;
        }

        basePosition += moveDirection * (moveSpeed * Time.deltaTime);

        float t = Time.time * swayFrequency + swayOffset;
        float sway  = Mathf.Sin(t) * swayAmplitude;
        float angle = Mathf.Sin(t) * rockAngle;

        Transform root = transform.root;
        Vector3 pivotTarget = new(
            basePosition.x + swayAxis.x * sway,
            basePosition.y + swayAxis.y * sway,
            root.position.z
        );

        // Posiciona root (com rotação zerada) para que o pivô fique em pivotTarget, depois gira em torno dele
        root.SetPositionAndRotation(pivotTarget - pivotOffset, Quaternion.identity);
        root.RotateAround(pivotTarget, Vector3.forward, angle);

        if (Camera.main != null)
        {
            Vector3 vp = Camera.main.WorldToViewportPoint(pivotTarget);
            bool onScreen = vp.x >= -0.1f && vp.x <= 1.1f && vp.y >= -0.1f && vp.y <= 1.1f;
            if (onScreen) hasEnteredScreen = true;
            else if (hasEnteredScreen) Destroy(gameObject);
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
using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    enum State { Entering, Attacking, Leaving }

    [Header("Movimento")]
    [Min(0f)] public float moveSpeed = 2f;
    public Vector2 moveDirection = Vector2.left;

    [Header("Parada para Ataque")]
    [Range(0f, 1f), Tooltip("Posição X na viewport onde o navio para (0 = borda esquerda, 1 = borda direita).")]
    public float stopViewportX = 0.6f;
    [Min(0f), Tooltip("Segundos que o navio fica parado atacando antes de continuar.")]
    public float attackDuration = 6f;
    [Range(0f, 1f), Tooltip("Distância (em viewport) antes do ponto de parada onde o navio começa a desacelerar.")]
    public float decelerationRange = 0.15f;
    [Range(0f, 1f), Tooltip("Velocidade mínima (fração de moveSpeed) ao se aproximar da parada, para garantir que o navio realmente chegue ao ponto.")]
    public float minSpeedFactor = 0.05f;
    [Min(0f), Tooltip("Tempo (segundos) para o navio acelerar até a velocidade máxima ao retomar o movimento após o ataque.")]
    public float accelerationTime = 1.5f;
    [Min(0f), Tooltip("Tempo (segundos) de espera após sair da tela antes de destruir o navio, para garantir que o sprite saia completamente.")]
    public float destroyDelay = 1f;

    [Header("Balanço")]
    [Min(0f), Tooltip("Altura do balanço em unidades de mundo.")]
    public float swayAmplitude = 0.15f;
    [Min(0f), Tooltip("Velocidade do balanço (ciclos por segundo).")]
    public float swayFrequency = 1.2f;

    [Header("Configurações de Dano")]
    [Min(1), Tooltip("Quantidade de tiros do canhão necessários para destruir este navio.")]
    public int maxHealth = 3;
    [Tooltip("Escuna normal: os dois false → sorteia incêndio ou vazamento aleatoriamente.")]
    public bool causesFloodOnly = false;
    [Tooltip("Navio de Guerra (Fase 4): causa incêndio E vazamento ao mesmo tempo.")]
    public bool causesBoth = false;

    [Header("Pontos de dano (Transforms no cenário do navio)")]
    // Arraste aqui 2 ou 3 pontos espalhados no navio onde incêndios/vazamentos podem surgir
    public Transform[] damagePoints;

    [Header("VFX")]
    public ParticleSystem explosionParticles;

    [Header("Áudio")]
    public AudioClip explosionSound;

    private int currentHealth;
    private AudioSource audioSource;
    private Vector2 basePosition;
    private Vector2 swayAxis;
    private float swayOffset;
    private State state = State.Entering;
    private float attackTimer;
    private float leavingTimer;
    private bool destroyScheduled;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        moveDirection = moveDirection.normalized;
        swayAxis = new Vector2(-moveDirection.y, moveDirection.x);
        swayOffset = Random.Range(0f, Mathf.PI * 2f);

        basePosition = (Vector2)transform.root.position;
    }

    void Update()
    {
        UpdateState();
        UpdateVisual();
    }

    void UpdateState()
    {
        if (Camera.main == null) return;

        Vector3 vp = Camera.main.WorldToViewportPoint(new Vector3(basePosition.x, basePosition.y, 0));

        switch (state)
        {
            case State.Entering:
                float speedFactor = 1f;
                float distanceToStop = vp.x - stopViewportX;
                if (decelerationRange > 0f && distanceToStop < decelerationRange)
                {
                    float t = Mathf.Clamp01(distanceToStop / decelerationRange);
                    speedFactor = Mathf.Max(t * t, minSpeedFactor);
                }
                basePosition += moveDirection * (moveSpeed * speedFactor * Time.deltaTime);
                if (vp.x <= stopViewportX)
                {
                    state = State.Attacking;
                    attackTimer = attackDuration;
                }
                break;

            case State.Attacking:
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    state = State.Leaving;
                    leavingTimer = 0f;
                }
                break;

            case State.Leaving:
                {
                    float leavingSpeedFactor = 1f;
                    if (accelerationTime > 0f)
                    {
                        leavingTimer += Time.deltaTime;
                        float t = Mathf.Clamp01(leavingTimer / accelerationTime);
                        leavingSpeedFactor = Mathf.Max(t * t, minSpeedFactor);
                    }
                    basePosition -= moveDirection * (moveSpeed * leavingSpeedFactor * Time.deltaTime);
                }
                if (!destroyScheduled && (vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f))
                {
                    destroyScheduled = true;
                    Destroy(transform.root.gameObject, destroyDelay);
                }
                break;
        }
    }

    void UpdateVisual()
    {
        float sway = Mathf.Sin(Time.time * swayFrequency + swayOffset) * swayAmplitude;
        Transform root = transform.root;
        root.position = new Vector3(
            basePosition.x + swayAxis.x * sway,
            basePosition.y + swayAxis.y * sway,
            root.position.z
        );
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
        float destroyAfter = 0f;

        // Som de explosão
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
            destroyAfter = explosionSound.length;
        }

        // Partícula de explosão: desparentamos para não sumir junto com o objeto
        if (explosionParticles != null)
        {
            ParticleSystem ps = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1f);
        }

        // Esconde o sprite para parecer destruído, mas mantém o objeto vivo para o som terminar
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Desativa colisão
        Collider2D col = GetComponentInChildren<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(transform.root.gameObject, destroyAfter);
    }
}
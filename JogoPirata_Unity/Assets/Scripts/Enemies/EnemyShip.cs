using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    enum State { Entering, Attacking, Leaving }

    [Header("Movimento")]
    [Min(0f)] public float moveSpeed = 2f;
    public Vector2 moveDirection = Vector2.left;

    [Header("Parada para Ataque")]
    [Range(0f, 1f)] public float stopViewportX = 0.6f;
    [Range(0f, 1f)] public float decelerationRange = 0.15f;
    [Range(0f, 1f)] public float minSpeedFactor = 0.05f;
    [Min(0f)] public float accelerationTime = 1.5f;
    [Min(0f)] public float destroyDelay = 1f;

    [Header("Tiro do Inimigo")]
    public GameObject projectilePrefab;
    public Transform[] firePoints;
    public Transform playerTarget;
    public bool useGuided = false;

    [Min(0)] public int tirosNormais = 3;
    [Min(0)] public int tirosRajada = 3;

    [Min(0f)] public float intervaloTiroNormal = 1.2f;
    [Min(0f)] public float pausaAntesRajada = 0.8f;
    [Min(0f)] public float intervaloRajada = 0.25f;

    [Header("Balanço")]
    [Min(0f)] public float swayAmplitude = 0.15f;
    [Min(0f)] public float swayFrequency = 1.2f;

    [Header("Configurações de Dano")]
    [Min(1)] public int maxHealth = 3;
    public bool causesFloodOnly = false;
    public bool causesBoth = false;

    [Header("Pontos de dano")]
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
    private float leavingTimer;
    private bool destroyScheduled;

    private int tirosNormaisDados = 0;
    private int tirosRajadaDados = 0;
    private float shootTimer = 0f;
    private bool emRajada = false;
    private bool ataqueFinalizado = false;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTarget = player.transform;
        }

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
                    Debug.Log("ENTROU EM ATAQUE!");

                    state = State.Attacking;
                    ReiniciarAtaque();
                }
                break;

            case State.Attacking:
                ControlarAtaque();

                if (ataqueFinalizado)
                {
                    Debug.Log("ATAQUE FINALIZADO!");

                    state = State.Leaving;
                    leavingTimer = 0f;
                }
                break;

            case State.Leaving:
                float leavingSpeedFactor = 1f;

                if (accelerationTime > 0f)
                {
                    leavingTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(leavingTimer / accelerationTime);
                    leavingSpeedFactor = Mathf.Max(t * t, minSpeedFactor);
                }

                basePosition -= moveDirection * (moveSpeed * leavingSpeedFactor * Time.deltaTime);

                if (!destroyScheduled && (vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f))
                {
                    destroyScheduled = true;
                    Destroy(transform.root.gameObject, destroyDelay);
                }
                break;
        }
    }

    void ReiniciarAtaque()
    {
        tirosNormaisDados = 0;
        tirosRajadaDados = 0;
        shootTimer = 0f;
        emRajada = false;
        ataqueFinalizado = false;
    }

    void ControlarAtaque()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer > 0f) return;

        if (tirosNormaisDados < tirosNormais)
        {
            AtirarAleatorio();
            tirosNormaisDados++;
            shootTimer = intervaloTiroNormal;
            return;
        }

        if (!emRajada)
        {
            Debug.Log("COMEÇOU PAUSA ANTES DA RAJADA!");

            emRajada = true;
            shootTimer = pausaAntesRajada;
            return;
        }

        if (tirosRajadaDados < tirosRajada)
        {
            int disparosFeitos = AtirarTodosCanhoesContando();
            tirosRajadaDados += disparosFeitos;
            shootTimer = intervaloRajada;
            return;
        }

        ataqueFinalizado = true;
    }

    void AtirarAleatorio()
    {
        Debug.Log("TENTOU ATIRAR NORMAL");

        if (!PodeAtirar()) return;

        Transform pontoEscolhido = firePoints[Random.Range(0, firePoints.Length)];
        CriarProjetil(pontoEscolhido);
    }

    int AtirarTodosCanhoesContando()
    {
        Debug.Log("TENTOU ATIRAR RAJADA");

        if (!PodeAtirar()) return 0;

        int disparos = 0;

        foreach (Transform ponto in firePoints)
        {
            if (tirosRajadaDados + disparos >= tirosRajada)
                break;

            CriarProjetil(ponto);
            disparos++;
        }

        return disparos;
    }

    bool PodeAtirar()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[EnemyShip] Falta Projectile Prefab!");
            return false;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogWarning("[EnemyShip] Falta Fire Points!");
            return false;
        }

        if (playerTarget == null)
        {
            Debug.LogWarning("[EnemyShip] Falta Player Target!");
            return false;
        }

        return true;
    }

    void CriarProjetil(Transform ponto)
    {
        if (ponto == null)
        {
            Debug.LogWarning("[EnemyShip] FirePoint vazio!");
            return;
        }

        GameObject tiro = Instantiate(projectilePrefab, ponto.position, Quaternion.identity);

        Projectile proj = tiro.GetComponent<Projectile>();

        if (proj != null)
        {
            proj.damagesEnemy = false;
            proj.isGuided = useGuided;

            if (useGuided)
            {
                proj.SetTarget(playerTarget);
            }
            else
            {
                Vector2 direcao = (playerTarget.position - ponto.position).normalized;
                float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                tiro.transform.rotation = Quaternion.Euler(0, 0, angulo);
            }
        }

        Debug.Log("PROJÉTIL CRIADO!");
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

    public void TakeHit()
    {
        currentHealth--;

        if (currentHealth <= 0)
            Morrer();
    }

    void Morrer()
    {
        float destroyAfter = 0f;

        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
            destroyAfter = explosionSound.length;
        }

        if (explosionParticles != null)
        {
            ParticleSystem ps = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1f);
        }

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponentInChildren<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(transform.root.gameObject, destroyAfter);
    }
}
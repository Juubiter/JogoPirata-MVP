using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    enum State { Entering, Attacking, Leaving }

    [Header("Movimento")]
    [Min(0f)] public float velocidadeDoNavio = 2f;
    public Vector2 direcaoMovimento = Vector2.left;

    [Header("Parada para Ataque")]
    [Range(0f, 1f)] public float posicaoDeParada = 0.6f;
    [Range(0f, 1f)] public float distanciaDesaceleracao = 0.15f;
    [Range(0f, 1f)] public float velocidadeMinima = 0.05f;
    [Min(0f)] public float tempoAceleracao = 1.5f;
    [Min(0f)] public float tempoParaDestruir = 1f;

    [Header("Tiro do Inimigo")]
    public GameObject prefabDaBala;
    public Transform[] canhoesDoNavio;
    public Transform alvoDoJogador;
    public bool tiroGuiado = false;

    [Min(0)] public int tirosNormais = 3;
    [Min(0)] public int tirosRajada = 3;

    [Min(0f)] public float intervaloTiroNormal = 1.2f;
    [Min(0f)] public float pausaAntesRajada = 0.8f;
    [Min(0f)] public float intervaloRajada = 0.25f;

    [Header("Balanço")]
    [Min(0f)] public float alturaBalanco = 0.15f;
    [Min(0f)] public float velocidadeBalanco = 1.2f;

    [Header("Configurações de Dano")]
    [Min(1)] public int vidaMaxima = 3;
    public bool causaApenasVazamento = false;
    public bool causaIncendioEVazamento = false;

    [Header("Pontos de Dano")]
    public Transform[] pontosDeDano;

    [Header("VFX")]
    public ParticleSystem particulasExplosao;

    [Header("Áudio")]
    public AudioClip somExplosao;

    private int vidaAtual;
    private AudioSource audioSource;

    private Vector2 posicaoBase;
    private Vector2 eixoBalanco;
    private float deslocamentoBalanco;

    private State estado = State.Entering;
    private float tempoSaindo;
    private bool destruicaoAgendada;

    private int tirosNormaisDados = 0;
    private int tirosRajadaDados = 0;
    private float temporizadorTiro = 0f;
    private bool emRajada = false;
    private bool ataqueFinalizado = false;

    void Start()
    {
        vidaAtual = vidaMaxima;
        audioSource = GetComponent<AudioSource>();

        if (alvoDoJogador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
                player = GameObject.Find("NavioPrincipal");

            if (player != null)
                alvoDoJogador = player.transform;
        }

        direcaoMovimento = direcaoMovimento.normalized;
        eixoBalanco = new Vector2(-direcaoMovimento.y, direcaoMovimento.x);
        deslocamentoBalanco = Random.Range(0f, Mathf.PI * 2f);

        posicaoBase = (Vector2)transform.root.position;
    }

    void Update()
    {
        AtualizarEstado();
        AtualizarVisual();
    }

    void AtualizarEstado()
    {
        if (Camera.main == null) return;

        Vector3 vp = Camera.main.WorldToViewportPoint(new Vector3(posicaoBase.x, posicaoBase.y, 0));

        switch (estado)
        {
            case State.Entering:
                float fatorVelocidade = 1f;
                float distanciaAteParada = vp.x - posicaoDeParada;

                if (distanciaDesaceleracao > 0f && distanciaAteParada < distanciaDesaceleracao)
                {
                    float t = Mathf.Clamp01(distanciaAteParada / distanciaDesaceleracao);
                    fatorVelocidade = Mathf.Max(t * t, velocidadeMinima);
                }

                posicaoBase += direcaoMovimento * (velocidadeDoNavio * fatorVelocidade * Time.deltaTime);

                if (vp.x <= posicaoDeParada)
                {
                    estado = State.Attacking;
                    ReiniciarAtaque();
                }
                break;

            case State.Attacking:
                ControlarAtaque();

                if (ataqueFinalizado)
                {
                    estado = State.Leaving;
                    tempoSaindo = 0f;
                }
                break;

            case State.Leaving:
                float fatorVelocidadeSaida = 1f;

                if (tempoAceleracao > 0f)
                {
                    tempoSaindo += Time.deltaTime;
                    float t = Mathf.Clamp01(tempoSaindo / tempoAceleracao);
                    fatorVelocidadeSaida = Mathf.Max(t * t, velocidadeMinima);
                }

                posicaoBase -= direcaoMovimento * (velocidadeDoNavio * fatorVelocidadeSaida * Time.deltaTime);

                if (!destruicaoAgendada && (vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f))
                {
                    destruicaoAgendada = true;
                    Destroy(transform.root.gameObject, tempoParaDestruir);
                }
                break;
        }
    }

    void ReiniciarAtaque()
    {
        tirosNormaisDados = 0;
        tirosRajadaDados = 0;
        temporizadorTiro = 0f;
        emRajada = false;
        ataqueFinalizado = false;
    }

    void ControlarAtaque()
    {
        temporizadorTiro -= Time.deltaTime;

        if (temporizadorTiro > 0f) return;

        if (tirosNormaisDados < tirosNormais)
        {
            AtirarAleatorio();
            tirosNormaisDados++;
            temporizadorTiro = intervaloTiroNormal;
            return;
        }

        if (!emRajada)
        {
            emRajada = true;
            temporizadorTiro = pausaAntesRajada;
            return;
        }

        if (tirosRajadaDados < tirosRajada)
        {
            int disparosFeitos = AtirarTodosCanhoesContando();
            tirosRajadaDados += disparosFeitos;
            temporizadorTiro = intervaloRajada;
            return;
        }

        ataqueFinalizado = true;
    }

    void AtirarAleatorio()
    {
        if (!PodeAtirar()) return;

        Transform canhaoEscolhido = canhoesDoNavio[Random.Range(0, canhoesDoNavio.Length)];
        CriarProjetil(canhaoEscolhido);
    }

    int AtirarTodosCanhoesContando()
    {
        if (!PodeAtirar()) return 0;

        int disparos = 0;

        foreach (Transform canhao in canhoesDoNavio)
        {
            if (tirosRajadaDados + disparos >= tirosRajada)
                break;

            CriarProjetil(canhao);
            disparos++;
        }

        return disparos;
    }

    bool PodeAtirar()
    {
        if (prefabDaBala == null)
        {
            Debug.LogWarning("[EnemyShip] Falta Prefab da Bala!");
            return false;
        }

        if (canhoesDoNavio == null || canhoesDoNavio.Length == 0)
        {
            Debug.LogWarning("[EnemyShip] Faltam os Canhões do Navio!");
            return false;
        }

        if (alvoDoJogador == null)
        {
            Debug.LogWarning("[EnemyShip] Falta o Alvo do Jogador!");
            return false;
        }

        return true;
    }

    void CriarProjetil(Transform canhao)
    {
        if (canhao == null)
        {
            Debug.LogWarning("[EnemyShip] Um canhão está vazio!");
            return;
        }

        GameObject tiro = Instantiate(prefabDaBala, canhao.position, Quaternion.identity);

        Projectile proj = tiro.GetComponent<Projectile>();

        if (proj != null)
        {
            proj.damagesEnemy = false;
            proj.isGuided = tiroGuiado;

            if (tiroGuiado)
            {
                proj.SetTarget(alvoDoJogador);
            }
            else
            {
                Vector2 direcao = (alvoDoJogador.position - canhao.position).normalized;
                float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                tiro.transform.rotation = Quaternion.Euler(0, 0, angulo);
            }
        }
    }

    void AtualizarVisual()
    {
        float balanco = Mathf.Sin(Time.time * velocidadeBalanco + deslocamentoBalanco) * alturaBalanco;
        Transform root = transform.root;

        root.position = new Vector3(
            posicaoBase.x + eixoBalanco.x * balanco,
            posicaoBase.y + eixoBalanco.y * balanco,
            root.position.z
        );
    }

    public void TakeHit()
    {
        vidaAtual--;

        if (vidaAtual <= 0)
            Morrer();
    }

    void Morrer()
    {
        float destruirDepois = 0f;

        if (somExplosao != null && audioSource != null)
        {
            audioSource.PlayOneShot(somExplosao);
            destruirDepois = somExplosao.length;
        }

        if (particulasExplosao != null)
        {
            ParticleSystem ps = Instantiate(particulasExplosao, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1f);
        }

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponentInChildren<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(transform.root.gameObject, destruirDepois);
    }
}
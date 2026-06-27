using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 12f;
    public bool isGuided = true;

    [Header("Dano")]
    public float damageAmount = 5f;
    public bool damagesEnemy = true;

    [Header("Buraco")]
    public GameObject buracoPrefab;
    public float tamanhoBuraco = 0.35f;
    public float variacaoPosicaoBuraco = 0.15f;

    [Header("Chance de Buraco")]
    [Range(0f, 100f)]
    public float chanceCriarBuraco = 35f;

    [Header("Incêndio")]
    public GameObject fogoPrefab;

    [Range(0f, 100f)]
    public float chanceIncendio = 20f;

    public float tamanhoFogo = 0.08f;

    [Header("Visual")]
    public bool useDepthScaling = true;
    public float depthScaleMin = 1.2f;
    public float depthScaleMax = 6f;
    public float maxDistance = 25f;

    private Transform target;
    private Vector3 posicaoInicial;
    private Vector2 velocidade;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Start()
    {
        posicaoInicial = transform.position;
        velocidade = transform.right * speed;
    }

    void Update()
    {
        if (isGuided)
            MoverGuiado();
        else
            MoverReto();

        if (useDepthScaling)
            AtualizarEscalaVisual();
    }

    void MoverGuiado()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
            Acertar();
    }

    void MoverReto()
    {
        transform.Translate(velocidade * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, posicaoInicial) > 100f)
            Destroy(gameObject);
    }

    void AtualizarEscalaVisual()
    {
        float distancia = Vector3.Distance(transform.position, posicaoInicial);
        float escala = Mathf.Lerp(depthScaleMin, depthScaleMax, Mathf.Clamp01(distancia / maxDistance));

        transform.localScale = Vector3.one * escala;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            -Mathf.Clamp01(distancia / maxDistance) * 2f
        );
    }

    void Acertar()
    {
        if (damagesEnemy && target != null)
        {
            EnemyShip navio = target.GetComponent<EnemyShip>();

            if (navio != null)
                navio.TakeHit();
        }
        else if (!damagesEnemy)
        {
            HUDManager hud = FindAnyObjectByType<HUDManager>();

            if (hud != null)
                hud.TakeDamage(damageAmount);

            if (Random.Range(0f, 100f) <= chanceCriarBuraco)
            {
                CriarBuracoNoNavio();
            }
            else
            {
                Debug.Log("Tiro acertou, mas não criou buraco.");
            }

            if (Random.Range(0f, 100f) <= chanceIncendio)
            {
                CriarIncendioNoNavio();
            }
            else
            {
                Debug.Log("Tiro acertou, mas não criou incêndio.");
            }
        }

        Destroy(gameObject);
    }

    void CriarBuracoNoNavio()
    {
        if (buracoPrefab == null)
        {
            Debug.LogWarning("BURACO PREFAB NÃO FOI COLOCADO!");
            return;
        }

        Vector3 posicaoBuraco = target != null ? target.position : transform.position;
        posicaoBuraco.z = 0f;

        GameObject buraco = Instantiate(buracoPrefab, posicaoBuraco, Quaternion.identity);
        buraco.name = "Buraco_Criado";

        GameObject navio = GameObject.FindGameObjectWithTag("Player");

        if (navio != null)
            buraco.transform.SetParent(navio.transform, true);

        buraco.transform.localScale = Vector3.one * 0.06f;

        Animator anim = buraco.GetComponent<Animator>();
        if (anim != null)
            anim.enabled = false;

        SpriteRenderer sr = buraco.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Fumaça";
            sr.sortingOrder = 9999;
        }

        Debug.Log("BURACO CRIADO EM: " + posicaoBuraco);

        ControleAgua controleAgua = FindFirstObjectByType<ControleAgua>();

        if (controleAgua != null)
            controleAgua.AdicionarBuraco();
    }

    void CriarIncendioNoNavio()
    {
        if (fogoPrefab == null)
        {
            Debug.LogWarning("FOGO PREFAB NÃO FOI COLOCADO!");
            return;
        }

        GameObject objetoPontos = GameObject.Find("PontosIncendio");

        if (objetoPontos == null)
        {
            Debug.LogWarning("PontosIncendio não encontrado na cena!");
            return;
        }

        Transform[] pontos = objetoPontos.GetComponentsInChildren<Transform>();

        if (pontos.Length <= 1)
        {
            Debug.LogWarning("Nenhum ponto filho encontrado em PontosIncendio!");
            return;
        }

        Transform pontoEscolhido = pontos[Random.Range(1, pontos.Length)];

        GameObject fogo = Instantiate(
            fogoPrefab,
            pontoEscolhido.position,
            Quaternion.identity
        );

        fogo.name = "Fogo_Criado";

        GameObject navio = GameObject.FindGameObjectWithTag("Player");

        if (navio != null)
            fogo.transform.SetParent(navio.transform, true);

        fogo.transform.localScale = Vector3.one * tamanhoFogo;

        SpriteRenderer sr = fogo.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sortingLayerName = "Fumaça";
            sr.sortingOrder = 10000;
        }

        Debug.Log("FOGO CRIADO EM: " + pontoEscolhido.name);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!damagesEnemy && collider.CompareTag("Player"))
        {
            Acertar();
        }
    }
}
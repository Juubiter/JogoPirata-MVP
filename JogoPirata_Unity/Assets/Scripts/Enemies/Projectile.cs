using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 12f;
    public bool isGuided = true;

    [Header("Dano")]
    public float damageAmount = 5f;
    public bool damagesEnemy = true;
    public TipoTiro tipoTiro = TipoTiro.Agua;

    [Header("Buraco de Água")]
    public GameObject buracoAguaPrefab;
    public float tamanhoBuracoAgua = 0.06f;

    [Header("Buraco de Fogo")]
    public GameObject buracoFogoPrefab;
    public float tamanhoBuracoFogo = 0.08f;

    [Header("Incêndio")]
    public GameObject fogoPrefab;
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

            switch (tipoTiro)
            {
                case TipoTiro.Agua:
                    Debug.Log("TIRO DE ÁGUA!");
                    CriarBuracoNoNavio();
                    break;

                case TipoTiro.Fogo:
                    Debug.Log("TIRO DE FOGO!");
                    CriarIncendioNoNavio();
                    break;
            }
        }

        Destroy(gameObject);
    }

    void CriarBuracoNoNavio()
    {
        if (buracoAguaPrefab == null)
        {
            Debug.LogWarning("BURACO DE ÁGUA PREFAB NÃO FOI COLOCADO!");
            return;
        }

        Vector3 posicaoBuraco = target != null ? target.position : transform.position;
        posicaoBuraco.z = 0f;

        GameObject buraco = Instantiate(buracoAguaPrefab, posicaoBuraco, Quaternion.identity);
        buraco.name = "Buraco_Agua_Criado";
        buraco.tag = "BuracoAgua";

        if (buraco.GetComponent<AlvoReservado>() == null)
            buraco.AddComponent<AlvoReservado>();

        GameObject navio = GameObject.FindGameObjectWithTag("Player");

        if (navio != null)
            buraco.transform.SetParent(navio.transform, true);

        buraco.transform.localScale = Vector3.one * tamanhoBuracoAgua;

        Animator anim = buraco.GetComponent<Animator>();
        if (anim != null)
            anim.enabled = false;

        SpriteRenderer[] renderers = buraco.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.sortingLayerName = "Fumaça";
            sr.sortingOrder = 9999;
        }

        Debug.Log("BURACO DE ÁGUA CRIADO EM: " + posicaoBuraco);

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

        if (target == null)
        {
            Debug.LogWarning("Target da bala está vazio. Não posso criar fogo.");
            return;
        }

        GameObject fogo = Instantiate(fogoPrefab, target.position, Quaternion.identity);
        fogo.name = "Fogo_Criado";
        fogo.tag = "Fogo";

        GameObject navio = GameObject.FindGameObjectWithTag("Player");

        if (navio != null)
            fogo.transform.SetParent(navio.transform, true);

        fogo.transform.localScale = Vector3.one * tamanhoFogo;

        if (fogo.GetComponent<AlvoReservado>() == null)
            fogo.AddComponent<AlvoReservado>();

        SpriteRenderer[] renderers = fogo.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.sortingLayerName = "Fumaça";
            sr.sortingOrder = 10000;
        }

        Debug.Log("FOGO CRIADO EM: " + fogo.transform.position);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!damagesEnemy && collider.CompareTag("Player"))
        {
            Acertar();
        }
    }
}
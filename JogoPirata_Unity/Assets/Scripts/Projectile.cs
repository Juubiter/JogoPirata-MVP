using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 12f;
    [Tooltip("true = guiado (segue alvo), false = reto (linha direta)")]
    public bool isGuided = true;

    [Header("Dano")]
    public float damageAmount = 5f;
    [Tooltip("true = causa dano em inimigo (EnemyShip), false = causa dano em jogador (Player)")]
    public bool damagesEnemy = true;

    [Header("Visual")]
    [Tooltip("Adiciona escala progressiva conforme distância (efeito de profundidade)")]
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
        velocidade.y -= 9.8f * Time.deltaTime;
        transform.Translate(velocidade * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, posicaoInicial) > 100f)
            Destroy(gameObject);
    }

    void AtualizarEscalaVisual()
    {
        float distancia = Vector3.Distance(transform.position, posicaoInicial);
        float escala = Mathf.Lerp(depthScaleMin, depthScaleMax, Mathf.Clamp01(distancia / maxDistance));
        transform.localScale = Vector3.one * escala;
        transform.position = new Vector3(transform.position.x, transform.position.y, -Mathf.Clamp01(distancia / maxDistance) * 2f);
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
            HUDManager hud = FindObjectOfType<HUDManager>();
            if (hud != null)
                hud.TakeDamage(damageAmount);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!damagesEnemy && collider.gameObject.CompareTag("Player"))
            Acertar();
    }
}

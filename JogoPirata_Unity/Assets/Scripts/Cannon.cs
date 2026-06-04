using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Configurações de Disparo")]
    public float fireInterval = 5f;
    public GameObject projectilePrefab;
    public Transform firePoint;         // ponta do canhão

    [Header("Pirata Operador")]
    // Arraste aqui a referência ao pirata que opera este canhão.
    // A Pessoa 4 (piratas) precisa ter um bool público "isWorking" no script Pirate.
    public Pirate pirateOperator;

    [Header("VFX e Áudio")]
    public ParticleSystem smokeParticles;
    public AudioClip fireSound;

    private float fireTimer;
    private AudioSource audioSource;

    void Start()
    {
        fireTimer = fireInterval;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Só dispara se houver pirata trabalhando no canhão
        if (pirateOperator == null || !pirateOperator.isWorking) return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            Atirar();
            fireTimer = fireInterval;
        }
    }

    void Atirar()
    {
        EnemyShip alvo = GetClosestEnemy();
        if (alvo == null) return;

        // Cria o projétil e manda na direção do inimigo
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            CannonProjectile projectile = proj.GetComponent<CannonProjectile>();
            if (projectile != null)
                projectile.SetTarget(alvo.transform);
        }

        // Fumaça do canhão
        if (smokeParticles != null)
            smokeParticles.Play();

        // Som de disparo
        if (fireSound != null && audioSource != null)
            audioSource.PlayOneShot(fireSound);
    }

    EnemyShip GetClosestEnemy()
    {
        EnemyShip[] inimigos = FindObjectsByType<EnemyShip>();
        EnemyShip maisProximo = null;
        float menorDist = Mathf.Infinity;

        foreach (EnemyShip inimigo in inimigos)
        {
            float dist = Vector2.Distance(transform.position, inimigo.transform.position);
            if (dist < menorDist)
            {
                menorDist = dist;
                maisProximo = inimigo;
            }
        }

        return maisProximo;
    }
}
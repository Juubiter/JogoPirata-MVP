using UnityEngine;
using System.Linq;

public class Weapon : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public Transform targetPoint;

    [Header("Disparo")]
    public float fireInterval = 5f;
    [Tooltip("Delay antes do primeiro tiro")]
    public float initialDelay = 0f;
    [Tooltip("true = guiado (Projectile.SetTarget), false = reto (Rigidbody2D.linearVelocity)")]
    public bool useGuided = false;

    [Header("VFX e Áudio")]
    public ParticleSystem particles;
    public AudioClip fireSound;

    private float fireTimer;
    private AudioSource audioSource;

    void Start()
    {
        fireTimer = fireInterval + initialDelay;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = GetComponentInChildren<AudioSource>();

        Debug.Log($"[Weapon] audioSource: {audioSource}, particles: {particles}");

        if (shootPoint == null)
            shootPoint = transform;

        if (targetPoint == null)
        {
            GameObject tp = GameObject.FindWithTag("CannonFirePoint");
            if (tp != null)
                targetPoint = tp.transform;
        }
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Fire();
            fireTimer = fireInterval;
        }
    }

    void Fire()
    {
        if (shootPoint == null || targetPoint == null || projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        Projectile projectile = proj.GetComponent<Projectile>();

        if (useGuided && projectile != null)
        {
            projectile.SetTarget(targetPoint);
        }
        else if (!useGuided && projectile != null)
        {
            Vector2 direcao = (targetPoint.position - shootPoint.position).normalized;
            float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, angulo);
        }

        if (particles != null) particles.Play();
        if (fireSound != null && audioSource != null) audioSource.PlayOneShot(fireSound);
    }
}

using UnityEngine;

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
    [Tooltip("Prefab do efeito (sprite/partícula) que será instanciado a cada tiro.")]
    public GameObject fireVFXPrefab;
    [Tooltip("Tempo até o efeito instanciado ser destruído.")]
    public float fireVFXDuration = 0.5f;
    public AudioClip fireSound;
    [Range(0f, 1f)] public float fireVolume = 1f;

    private float fireTimer;

    void Start()
    {
        fireTimer = fireInterval + initialDelay;

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

        if (fireVFXPrefab != null)
        {
            // Z negativo = à frente do cenário (o shootPoint fica em Z=1, no fundo).
            Vector3 vfxPos = shootPoint.position;
            vfxPos.z = -1f;

            // Mantém a rotação ORIGINAL do prefab — importante para ParticleSystem,
            // que pode estar tiltado para emitir no plano da câmera.
            GameObject vfx = Instantiate(fireVFXPrefab, vfxPos, fireVFXPrefab.transform.rotation);

            float vida = fireVFXDuration;
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                vida = ps.main.duration + ps.main.startLifetime.constantMax;
            }

            Destroy(vfx, vida);
        }

        if (fireSound != null)
        {
            GameObject sfxObj = new GameObject("SFX_Cannon");
            sfxObj.transform.position = shootPoint.position;
            AudioSource sfx = sfxObj.AddComponent<AudioSource>();
            sfx.clip = fireSound;
            sfx.volume = fireVolume;
            sfx.spatialBlend = 0f;
            sfx.Play();
            Destroy(sfxObj, fireSound.length);
        }
    }
}

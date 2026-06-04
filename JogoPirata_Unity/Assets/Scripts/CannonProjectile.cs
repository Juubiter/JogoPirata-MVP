using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    public float speed = 12f;

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        // Se o alvo sumiu (navio já morreu antes do projétil chegar), destrói o projétil
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move em direção ao alvo
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Chegou perto o suficiente → acertou
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
            Acertar();
    }

    void Acertar()
    {
        if (target != null)
        {
            EnemyShip navio = target.GetComponent<EnemyShip>();
            if (navio != null)
                navio.TakeHit();
        }

        Destroy(gameObject);
    }
}
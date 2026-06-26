using UnityEngine;

// Faz o personagem subir/descer uma escada vertical e troca para a
// animação de escada (clip EscadaNPC1) enquanto está nela.
// A escada precisa de um Collider2D com "Is Trigger" marcado e a Tag "Escada".
[RequireComponent(typeof(Animator))]
public class SubirEscada : MonoBehaviour
{
    [Tooltip("Velocidade ao subir/descer a escada.")]
    public float velocidadeEscada = 2f;

    [Tooltip("Nome do parâmetro bool no Animator que liga a animação de escada.")]
    public string parametroSubindo = "Subindo";

    [Tooltip("Tag do collider da escada.")]
    public string tagEscada = "Escada";

    private Animator animator;
    private Rigidbody2D rb;
    private float gravidadeOriginal;
    private bool naEscada;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            gravidadeOriginal = rb.gravityScale;
        }
    }

    void Update()
    {
        if (!naEscada)
        {
            return;
        }

        // Entrada vertical: W/S ou setas cima/baixo
        float vertical = Input.GetAxisRaw("Vertical");

        // Move na vertical
        Vector3 movimento = Vector3.up * vertical * velocidadeEscada * Time.deltaTime;
        transform.position += movimento;

        // Liga a animação de escada e congela no frame atual quando parado
        animator.SetBool(parametroSubindo, true);
        animator.speed = Mathf.Abs(vertical) > 0.01f ? 1f : 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagEscada))
        {
            return;
        }

        naEscada = true;

        // Desliga a gravidade pra não escorregar enquanto sobe
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagEscada))
        {
            return;
        }

        naEscada = false;
        animator.SetBool(parametroSubindo, false);
        animator.speed = 1f;

        if (rb != null)
        {
            rb.gravityScale = gravidadeOriginal;
        }
    }
}

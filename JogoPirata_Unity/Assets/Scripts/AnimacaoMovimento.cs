using UnityEngine;

// Detecta se o objeto está se movendo e avisa o Animator,
// trocando entre a animação parado (Idle) e andando (Run).
// Funciona com qualquer script que mova o transform (ex: MoverNpc).
[RequireComponent(typeof(Animator))]
public class AnimacaoMovimento : MonoBehaviour
{
    [Tooltip("Nome do parâmetro bool no Animator que liga a animação de andar.")]
    public string parametroAndando = "Andando";

    [Tooltip("Velocidade mínima para considerar que está andando.")]
    public float limiteMovimento = 0.01f;

    [Tooltip("Vira o sprite na horizontal de acordo com a direção do movimento.")]
    public bool virarSprite = true;

    private Animator animator;
    private Vector3 posicaoAnterior;

    void Start()
    {
        animator = GetComponent<Animator>();
        posicaoAnterior = transform.position;
    }

    void Update()
    {
        // Quanto o objeto andou desde o último frame
        Vector3 deslocamento = transform.position - posicaoAnterior;
        float velocidade = deslocamento.magnitude / Time.deltaTime;

        bool estaAndando = velocidade > limiteMovimento;
        animator.SetBool(parametroAndando, estaAndando);

        // Vira o sprite para o lado que está se movendo
        if (virarSprite && Mathf.Abs(deslocamento.x) > 0.0001f)
        {
            float escalaX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(
                deslocamento.x < 0 ? -escalaX : escalaX,
                transform.localScale.y,
                transform.localScale.z
            );
        }

        posicaoAnterior = transform.position;
    }
}

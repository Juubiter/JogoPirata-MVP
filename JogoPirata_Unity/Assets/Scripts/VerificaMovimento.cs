using UnityEngine;

public class VerificaMovimento : MonoBehaviour
{
    private Animator meuAnimator;
    private Vector2 posicaoAnterior;
    
    // Uma margem minúscula para ignorar tremulações microscópicas do motor físico
    private float limiteMovimento = 0.001f; 

    void Start()
    {
        // Pega o componente Animator anexado ao boneco
        meuAnimator = GetComponent<Animator>();
        
        // Grava a posição inicial no primeiro momento
        posicaoAnterior = transform.position;
    }

    void Update()
    {
        // Calcula a distância entre a posição de agora e a posição do frame anterior
        float distanciaMovimentada = Vector2.Distance(transform.position, posicaoAnterior);

        // Se ele andou uma distância maior que o limite, significa que está em movimento
        bool estaSeMovendo = distanciaMovimentada > limiteMovimento;

        // Envia o true ou false para o parâmetro "andando" lá no Animator
        // IMPORTANTE: O nome aqui entre aspas tem que ser exatamente igual ao do Animator
        meuAnimator.SetBool("andando", estaSeMovendo);

        // Atualiza a 'posicaoAnterior' com a posição atual para ser usada no próximo frame
        posicaoAnterior = transform.position;
    }
}
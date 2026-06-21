using UnityEngine;

public class MoverNpc : MonoBehaviour
{
    public float velocidade = 2f;
    
    // As caixinhas que vão aparecer na Unity para você colocar os pontos
    public Transform pontoA;
    public Transform pontoB;
    
    private Transform alvoAtual;

    void Start()
    {
        // Define que o primeiro destino é o Ponto A
        alvoAtual = pontoA; 
    }

    void Update()
    {
        // TRAVA DE SEGURANÇA: Se a caixinha do Ponto A ou B estiver vazia na Unity, 
        // ele para de ler o código aqui para não dar erro!
        if (pontoA == null || pontoB == null) 
        {
            return; 
        }

        // Move o boneco na direção do alvo
        transform.position = Vector3.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

        // Se chegar bem perto do alvo, ele inverte a direção
        if (Vector3.Distance(transform.position, alvoAtual.position) < 0.1f)
        {
            if (alvoAtual == pontoA)
            {
                alvoAtual = pontoB;
            }
            else
            {
                alvoAtual = pontoA;
            }
        }
    }
}

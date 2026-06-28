using UnityEngine;

public class MenuAcoes : MonoBehaviour
{
    private Transform alvo; // Guarda a referência do NPC
    
    // Opcional: Um ajuste para o menu não ficar exatamente em cima do sprite do NPC
    public Vector3 offset = new Vector3(0, 1.5f, 0); 

    public void Abrir(Transform npcTransform)
    {
        alvo = npcTransform;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // O Update roda todo frame. Se houver um alvo, ele recalcula a posição na tela.
        if (alvo != null)
        {
            Vector3 posTela = Camera.main.WorldToScreenPoint(alvo.position + offset);
            transform.position = posTela;
        }
    }

    public void Fechar()
    {
        alvo = null;
        gameObject.SetActive(false);
    }
}
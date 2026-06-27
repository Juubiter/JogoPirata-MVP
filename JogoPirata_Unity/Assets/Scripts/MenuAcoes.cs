using UnityEngine;

public class MenuAcoes : MonoBehaviour
{
    public void Abrir(Vector3 posicao)
    {
        Vector3 posTela = Camera.main.WorldToScreenPoint(posicao);

        transform.position = posTela;

        gameObject.SetActive(true);
    }

    public void Fechar()
    {
        gameObject.SetActive(false);
    }
}
using UnityEngine;

public class MenuAcoes : MonoBehaviour
{
    public GameObject menu;


    public void Abrir(Vector3 posicao)
    {
        menu.transform.position = posicao;
        menu.SetActive(true);
    }


    public void Fechar()
    {
        menu.SetActive(false);
    }
}
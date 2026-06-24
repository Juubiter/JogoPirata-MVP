using UnityEngine;


public class PirateController : MonoBehaviour
{

    public GameObject highlight;



    public void Selecionar()
    {
        highlight.SetActive(true);

        Debug.Log("PIRATA SELECIONADO");
    }



    public void Deselecionar()
    {
        highlight.SetActive(false);

        Debug.Log("PIRATA DESSELECIONADO");
    }



    public void ReceberOrdem(Ordem ordem)
    {
        Debug.Log(
            "Pirata recebeu ordem: " + ordem
        );
    }

}
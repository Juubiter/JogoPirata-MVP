using UnityEngine;

public class PirateController : MonoBehaviour
{
    public Ordem ordemAtual = Ordem.Nenhuma;

    public GameObject destaqueLaranja;

    public void Selecionar()
    {
        destaqueLaranja.SetActive(true);

        Debug.Log("Pirata selecionado!");
    }

    public void Deselecionar()
    {
        destaqueLaranja.SetActive(false);
    }

    public void ReceberOrdem(Ordem novaOrdem)
    {
        ordemAtual = novaOrdem;

        Debug.Log("Nova ordem: " + novaOrdem);
    }
}
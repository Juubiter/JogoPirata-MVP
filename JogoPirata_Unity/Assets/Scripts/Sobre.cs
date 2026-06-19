using UnityEngine;

public class AbrirFecharPainel : MonoBehaviour
{
    public GameObject painel;

    // Objetos que vão sumir quando o painel abrir
    public GameObject[] objetosParaOcultar;

    // Abre o painel
    public void AbrirPainel()
    {
        painel.SetActive(true);

        foreach (GameObject obj in objetosParaOcultar)
        {
            obj.SetActive(false);
        }
    }

    // Fecha o painel
    public void FecharPainel()
    {
        painel.SetActive(false);

        foreach (GameObject obj in objetosParaOcultar)
        {
            obj.SetActive(true);
        }
    }
}
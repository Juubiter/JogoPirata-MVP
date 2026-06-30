using UnityEngine;

public enum OrdemPirata
{
    ApagarFogo,
    TamparBuraco,
    TirarAgua,
    AtirarCanhao
}

public class GerenciadorMenuAcoes : MonoBehaviour
{
    public static GerenciadorMenuAcoes Instancia;

    [Header("Pirata Ativo")]
    public PirateController pirataSelecionado;

    void Awake()
    {
        Instancia = this;
    }

    public void ClicouBotaoIncendio()
    {
        Debug.Log("BOTÃO FOGO CLICADO");

        if (pirataSelecionado != null)
        {
            Debug.Log("Enviando ordem de apagar fogo para: " + pirataSelecionado.name);
            pirataSelecionado.ReceberOrdem(OrdemPirata.ApagarFogo);
        }
        else
        {
            Debug.LogWarning("Nenhum pirata selecionado para apagar fogo.");
        }
    }

    public void ClicouBotaoBuraco()
    {
        Debug.Log("BOTÃO BURACO CLICADO");

        if (pirataSelecionado != null)
        {
            Debug.Log("Enviando ordem de tampar buraco para: " + pirataSelecionado.name);
            pirataSelecionado.ReceberOrdem(OrdemPirata.TamparBuraco);
        }
        else
        {
            Debug.LogWarning("Nenhum pirata selecionado para tampar buraco.");
        }
    }

    public void ClicouBotaoAgua()
    {
        Debug.Log("BOTÃO BALDE CLICADO");

        if (pirataSelecionado != null)
        {
            Debug.Log("Enviando ordem de tirar água para: " + pirataSelecionado.name);
            pirataSelecionado.ReceberOrdem(OrdemPirata.TirarAgua);
        }
        else
        {
            Debug.LogWarning("Nenhum pirata selecionado para tirar água.");
        }
    }

    public void ClicouBotaoCanhao()
    {
        Debug.Log("BOTÃO CANHÃO CLICADO");

        if (pirataSelecionado != null)
        {
            Debug.Log("Enviando ordem de atirar para: " + pirataSelecionado.name);
            pirataSelecionado.ReceberOrdem(OrdemPirata.AtirarCanhao);
        }
        else
        {
            Debug.LogWarning("Nenhum pirata selecionado para atirar canhão.");
        }
    }
}
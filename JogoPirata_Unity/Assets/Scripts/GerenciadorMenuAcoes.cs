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
        if (pirataSelecionado != null)
            pirataSelecionado.ReceberOrdem(OrdemPirata.ApagarFogo);
        else
            Debug.LogWarning("Nenhum pirata selecionado para apagar fogo.");
    }

    public void ClicouBotaoBuraco()
    {
        if (pirataSelecionado != null)
            pirataSelecionado.ReceberOrdem(OrdemPirata.TamparBuraco);
        else
            Debug.LogWarning("Nenhum pirata selecionado para tampar buraco.");
    }

    public void ClicouBotaoAgua()
    {
        if (pirataSelecionado != null)
            pirataSelecionado.ReceberOrdem(OrdemPirata.TirarAgua);
        else
            Debug.LogWarning("Nenhum pirata selecionado para tirar água.");
    }

    public void ClicouBotaoCanhao()
    {
        if (pirataSelecionado != null)
            pirataSelecionado.ReceberOrdem(OrdemPirata.AtirarCanhao);
        else
            Debug.LogWarning("Nenhum pirata selecionado para atirar canhão.");
    }
}
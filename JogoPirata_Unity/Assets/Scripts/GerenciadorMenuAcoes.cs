using UnityEngine;

public enum OrdemPirata { ApagarFogo, TamparBuraco, TirarAgua, AtirarCanhao }

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
    }

    public void ClicouBotaoBuraco()
    {
        if (pirataSelecionado != null) 
            pirataSelecionado.ReceberOrdem(OrdemPirata.TamparBuraco);
    }

    public void ClicouBotaoAgua()
    {
        if (pirataSelecionado != null) 
            pirataSelecionado.ReceberOrdem(OrdemPirata.TirarAgua);
    }

    public void ClicouBotaoCanhao()
    {
        if (pirataSelecionado != null) 
            pirataSelecionado.ReceberOrdem(OrdemPirata.AtirarCanhao);
    }
}
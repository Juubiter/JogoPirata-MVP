using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public MenuAcoes menuAcoes;

    private PirateController pirataSelecionado;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            VerificarClique();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelarSelecao();
        }
    }

    void VerificarClique()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null)
            return;

        Debug.Log("Clicou em: " + hit.collider.name);

        // Verifica se clicou em um pirata
        PirateController pirata = hit.collider.GetComponent<PirateController>();

        if (pirata != null)
        {
            if (pirataSelecionado != null)
            {
                pirataSelecionado.Deselecionar();
            }

            pirataSelecionado = pirata;

            pirataSelecionado.Selecionar();

            menuAcoes.Abrir(pirata.transform);

            return;
        }

        // Futuramente servir� para clicar em objetos do cen�rio
        ObjetoInterativo objeto = hit.collider.GetComponent<ObjetoInterativo>();

        if (objeto != null && pirataSelecionado != null)
        {
            objeto.ReceberOrdem(pirataSelecionado);

            FinalizarSelecao();
        }
    }

    void CancelarSelecao()
    {
        FinalizarSelecao();

        Debug.Log("Sele��o cancelada");
    }

    // Fecha o menu e remove a sele��o do pirata
    void FinalizarSelecao()
    {
        if (pirataSelecionado != null)
        {
            pirataSelecionado.Deselecionar();
            pirataSelecionado = null;
        }

        menuAcoes.Fechar();
    }

    public void OrdemCanhao()
    {
        if (pirataSelecionado == null)
            return;

        pirataSelecionado.ReceberOrdem(OrdemPirata.AtirarCanhao);

        FinalizarSelecao();
    }

    public void OrdemIncendio()
    {
        if (pirataSelecionado == null)
            return;

        pirataSelecionado.ReceberOrdem(OrdemPirata.ApagarFogo);

        FinalizarSelecao();
    }

    public void OrdemAgua()
    {
        if (pirataSelecionado == null)
            return;

        pirataSelecionado.ReceberOrdem(OrdemPirata.TirarAgua);

        FinalizarSelecao();
    }

    public void OrdemBuraco()
    {
        if (pirataSelecionado == null)
            return;

        pirataSelecionado.ReceberOrdem(OrdemPirata.TamparBuraco);

        FinalizarSelecao();
    }
}
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public GameObject menuOrdens;

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
        Vector2 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit =
            Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            PirateController pirata =
                hit.collider.GetComponent<PirateController>();

            if (pirata != null)
            {
                if (pirataSelecionado != null)
                {
                    pirataSelecionado.Deselecionar();
                }

                pirataSelecionado = pirata;

                pirataSelecionado.Selecionar();

                menuOrdens.SetActive(true);
            }
        }
    }

    void CancelarSelecao()
    {
        if (pirataSelecionado != null)
        {
            pirataSelecionado.Deselecionar();
            pirataSelecionado = null;
        }

        menuOrdens.SetActive(false);

        Debug.Log("Seleção cancelada");
    }

    public void OrdemCanhao()
    {
        pirataSelecionado.ReceberOrdem(Ordem.Canhao);
        menuOrdens.SetActive(false);
    }

    public void OrdemIncendio()
    {
        pirataSelecionado.ReceberOrdem(Ordem.Incendio);
        menuOrdens.SetActive(false);
    }

    public void OrdemAgua()
    {
        pirataSelecionado.ReceberOrdem(Ordem.Agua);
        menuOrdens.SetActive(false);
    }

    public void OrdemBuraco()
    {
        pirataSelecionado.ReceberOrdem(Ordem.Buraco);
        menuOrdens.SetActive(false);
    }
}
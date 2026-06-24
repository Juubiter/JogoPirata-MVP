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
        Vector2 mousePos =
        Camera.main.ScreenToWorldPoint(Input.mousePosition);


        RaycastHit2D hit =
        Physics2D.Raycast(mousePos, Vector2.zero);



        if (hit.collider == null)
        {
            return;
        }



        Debug.Log("Clicou em: " + hit.collider.name);



        // Primeiro verifica se clicou em um pirata

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


            // abre a roleta em cima do pirata

            menuAcoes.Abrir(
            pirata.transform.position
            );


            return;
        }



        // Se futuramente clicar em objetos do mapa
        // fica preparado aqui


        ObjetoInterativo objeto =
        hit.collider.GetComponent<ObjetoInterativo>();


        if (objeto != null && pirataSelecionado != null)
        {
            objeto.ReceberOrdem(pirataSelecionado);

            menuAcoes.Fechar();
        }

    }





    void CancelarSelecao()
    {

        if (pirataSelecionado != null)
        {
            pirataSelecionado.Deselecionar();

            pirataSelecionado = null;
        }


        menuAcoes.Fechar();


        Debug.Log("Seleção cancelada");
    }





    public void OrdemCanhao()
    {
        if (pirataSelecionado == null)
            return;


        pirataSelecionado.ReceberOrdem(Ordem.Canhao);

        menuAcoes.Fechar();
    }





    public void OrdemIncendio()
    {
        if (pirataSelecionado == null)
            return;


        pirataSelecionado.ReceberOrdem(Ordem.Incendio);

        menuAcoes.Fechar();
    }





    public void OrdemAgua()
    {
        if (pirataSelecionado == null)
            return;


        pirataSelecionado.ReceberOrdem(Ordem.Agua);

        menuAcoes.Fechar();
    }





    public void OrdemBuraco()
    {
        if (pirataSelecionado == null)
            return;


        pirataSelecionado.ReceberOrdem(Ordem.Buraco);

        menuAcoes.Fechar();
    }
}
using UnityEngine;

public class PirateController : MonoBehaviour
{
    public Ordem ordemAtual = Ordem.Nenhuma;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Selecionar()
    {
        spriteRenderer.color = new Color(1f, 0.7f, 0f);

        Debug.Log("Pirata selecionado!");
    }

    public void Deselecionar()
    {
        spriteRenderer.color = Color.white;
    }

    public void ReceberOrdem(Ordem novaOrdem)
    {
        ordemAtual = novaOrdem;

        Debug.Log("Nova ordem: " + novaOrdem);
    }
}
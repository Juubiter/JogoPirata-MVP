using UnityEngine;

public class VidaNavioInimigo : MonoBehaviour
{
    [Header("Vida visual por caveiras")]
    public GameObject[] caveiras;

    private int danoAtual = 0;
    private bool derrotado = false;

    void Start()
    {
        AtualizarCaveiras();
    }

    public void ReceberDano()
    {
        if (derrotado)
            return;

        danoAtual++;
        AtualizarCaveiras();

        Debug.Log("Navio inimigo recebeu dano: " + danoAtual + "/" + caveiras.Length);

        if (danoAtual >= caveiras.Length)
        {
            derrotado = true;
            DerrotarNavio();
        }
    }

    void AtualizarCaveiras()
    {
        for (int i = 0; i < caveiras.Length; i++)
        {
            if (caveiras[i] != null)
                caveiras[i].SetActive(i < danoAtual);
        }
    }

    void DerrotarNavio()
    {
        Debug.Log("NAVIO INIMIGO DERROTADO!");

        HUDManager hud = FindAnyObjectByType<HUDManager>();

        if (hud != null)
            hud.Vitoria();

        Destroy(gameObject);
    }
}
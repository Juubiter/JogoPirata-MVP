using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerd : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject painel;

    void Start()
    {
        Time.timeScale = 0f;
    }

    public void IniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Fase1");
    }

    public void AbrirPainel()
    {
        painel.SetActive(true);
    }

    public void FecharPainel()
    {
        painel.SetActive(false);
    }

    public void SairJogo()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }
}
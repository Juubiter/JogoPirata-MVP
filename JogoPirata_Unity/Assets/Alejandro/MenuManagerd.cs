using UnityEngine;

public class MenuManagerd : MonoBehaviour
{
    public GameObject painelControles;
    public GameObject menuPrincipal;

    void Start()
{
    Time.timeScale = 0f;
}

   public void IniciarJogo()
{
    menuPrincipal.SetActive(false);
    Time.timeScale = 1f;
}

    public void AbrirControles()
    {
        painelControles.SetActive(true);
    }

    public void FecharControles()
    {
        painelControles.SetActive(false);
    }

    public void SairJogo()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }
    
}
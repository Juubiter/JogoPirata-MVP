using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void IniciarJogo()
    {
        Debug.Log("BOTÃO CLICADO!");
        SceneManager.LoadScene("Fase1");
    }

    public void SairJogo()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
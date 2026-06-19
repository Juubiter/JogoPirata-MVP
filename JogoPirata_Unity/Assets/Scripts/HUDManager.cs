using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public GameObject telaVitoria;
    public GameObject telaDerrota;
    public GameObject painelPause;

    public Slider barraVida;
    public Slider barraAgua;
    public Slider sliderVolume;

    private float vida = 100f;
    private float agua = 0f;
    private float tempo = 0f;

    public float tempoParaVencer = 30f;

    void Start()
    {
        barraVida.value = vida;
        barraAgua.value = agua;

        telaVitoria.SetActive(false);
        telaDerrota.SetActive(false);
        painelPause.SetActive(false);

        sliderVolume.value = AudioListener.volume;
    }

    void Update()
    {
        tempo += Time.deltaTime;

        // Abrir Pause com ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AbrirPause();
        }

        if (tempo >= tempoParaVencer)
        {
            telaVitoria.SetActive(true);
            Time.timeScale = 0f;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            vida = Mathf.Max(0, vida - 10);
            barraVida.value = vida;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            agua = Mathf.Min(100, agua + 10);
            barraAgua.value = agua;
        }

        if (vida <= 0 || agua >= 100)
        {
            telaDerrota.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // ===== PAUSE =====

    public void AbrirPause()
    {
        painelPause.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinuarJogo()
    {
        painelPause.SetActive(false);
        Time.timeScale = 1f;
    }


   public void VoltarMenu()
{
    Time.timeScale = 1f;
    Debug.Log("Voltando ao menu");
    SceneManager.LoadScene("Menu");
}

    // ===== DERROTA E VITÓRIA =====

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;

        vida = 100f;
        agua = 0f;
        tempo = 0f;

        barraVida.value = vida;
        barraAgua.value = agua;

        telaDerrota.SetActive(false);
        telaVitoria.SetActive(false);
    }

    public void JogarNovamente()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public GameObject telaVitoria;
    public GameObject telaDerrota;
    public GameObject painelPause;

    // Objetos que somem no pause
    public GameObject[] objetosParaOcultar;

    public Slider barraVida;
    public Slider barraAgua;

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

        AudioListener.volume = 1f;
    }

    void Update()
    {
        tempo += Time.deltaTime;

        // Abrir pause com ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!painelPause.activeSelf)
                AbrirPause();
            else
                ContinuarJogo();
        }

        // Vitória
        if (tempo >= tempoParaVencer)
        {
            telaVitoria.SetActive(true);
            Time.timeScale = 0f;
        }

        // Teste de dano
        if (Input.GetKeyDown(KeyCode.H))
        {
            vida = Mathf.Max(0, vida - 10);
            barraVida.value = vida;
        }

        // Teste de água
        if (Input.GetKeyDown(KeyCode.J))
        {
            agua = Mathf.Min(100, agua + 10);
            barraAgua.value = agua;
        }

        // Derrota
        if (vida <= 0 || agua >= 100)
        {
            telaDerrota.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // ===== PAUSE =====

    public void AbrirPause()
    {
        if (telaVitoria.activeSelf || telaDerrota.activeSelf)
            return;

        painelPause.SetActive(true);

        foreach (GameObject obj in objetosParaOcultar)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    public void ContinuarJogo()
    {
        painelPause.SetActive(false);

        foreach (GameObject obj in objetosParaOcultar)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        Time.timeScale = 1f;
    }

    // ===== VIDA E ÁGUA =====

    public void TakeDamage(float amount)
    {
        vida = Mathf.Max(0, vida - amount);
        barraVida.value = vida;
    }

    public void TakeWaterDamage(float amount)
    {
        agua = Mathf.Min(100, agua + amount);
        barraAgua.value = agua;
    }

    // ===== MENU =====

    public void VoltarMenu()
    {
        Time.timeScale = 1f;
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

public void ProximaFase()
{
    Time.timeScale = 1f;

    int faseAtual = SceneManager.GetActiveScene().buildIndex;
    SceneManager.LoadScene(faseAtual + 1);
}
}
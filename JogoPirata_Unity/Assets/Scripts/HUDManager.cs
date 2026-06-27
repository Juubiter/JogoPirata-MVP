using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public GameObject telaVitoria;
    public GameObject telaDerrota;
    public GameObject painelPause;

    public GameObject[] objetosParaOcultar;

    public Slider barraVida;
    public Slider barraAgua;

    private float vida;
    private float agua;
    private float tempo;
    private bool jogoFinalizado;

    public float tempoParaVencer = 100f;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        ReiniciarValores();
    }

    void Update()
    {
        if (jogoFinalizado)
            return;

        tempo += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (painelPause != null && painelPause.activeSelf)
                ContinuarJogo();
            else
                AbrirPause();
        }

        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10f);

        if (Input.GetKeyDown(KeyCode.J))
            TakeWaterDamage(10f);

        if (vida <= 0 || agua >= 100)
        {
            Derrota();
            return;
        }

        if (tempo >= tempoParaVencer)
        {
            Vitoria();
            return;
        }
    }

    void ReiniciarValores()
    {
        Time.timeScale = 1f;

        vida = 100f;
        agua = 0f;
        tempo = 0f;
        jogoFinalizado = false;

        if (barraVida != null)
            barraVida.value = vida;

        if (barraAgua != null)
            barraAgua.value = agua;

        if (telaVitoria != null)
            telaVitoria.SetActive(false);

        if (telaDerrota != null)
            telaDerrota.SetActive(false);

        if (painelPause != null)
            painelPause.SetActive(false);

        MostrarObjetosHUD(true);

        AudioListener.volume = 1f;
    }

    void Vitoria()
    {
        jogoFinalizado = true;

        if (telaVitoria != null)
            telaVitoria.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Derrota()
    {
        jogoFinalizado = true;

        if (telaDerrota != null)
            telaDerrota.SetActive(true);

        Time.timeScale = 0f;
    }

    public void AbrirPause()
    {
        if (jogoFinalizado)
            return;

        if (painelPause != null)
            painelPause.SetActive(true);

        MostrarObjetosHUD(false);
        Time.timeScale = 0f;
    }

    public void ContinuarJogo()
    {
        if (painelPause != null)
            painelPause.SetActive(false);

        MostrarObjetosHUD(true);
        Time.timeScale = 1f;
    }

    void MostrarObjetosHUD(bool mostrar)
    {
        foreach (GameObject obj in objetosParaOcultar)
        {
            if (obj == null)
                continue;

            obj.SetActive(mostrar);
        }
    }

    public void TakeDamage(float amount)
    {
        if (jogoFinalizado)
            return;

        vida = Mathf.Max(0, vida - amount);

        if (barraVida != null)
            barraVida.value = vida;
    }

    public void TakeWaterDamage(float amount)
    {
        if (jogoFinalizado)
            return;

        agua = Mathf.Min(100, agua + amount);

        if (barraAgua != null)
            barraAgua.value = agua;
    }

    public void VoltarMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void JogarNovamente()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ProximaFase()
    {
        Time.timeScale = 1f;

        int proximaFase = SceneManager.GetActiveScene().buildIndex + 1;

        if (proximaFase < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(proximaFase);
        else
            SceneManager.LoadScene("Menu");
    }
}
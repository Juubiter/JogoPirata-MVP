using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HUDManager : MonoBehaviour
{
    public GameObject telaVitoria;
private float tempo = 0f;
public float tempoParaVencer = 30f;
    public GameObject telaDerrota;
    public Slider barraVida;
    public Slider barraAgua;

    private float vida = 100f;
    private float agua = 0f;

    void Start()
    {
        barraVida.value = vida;
        barraAgua.value = agua;
    }

    void Update()
{
    tempo += Time.deltaTime;

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
using UnityEngine;
using UnityEngine.UI;

public class ControleAgua : MonoBehaviour
{
    [Header("Configurações de Nível")]
    public int nivelAtual = 0;
    public int nivelMaximo = 4;

    [Header("Vazamentos")]
    public int buracosAbertos = 0;
    public float tempoParaSubirNivel = 15f;

    private float contadorAgua = 0f;

    [Header("Referências da Interface (HUD)")]
    public Slider barraAguaSlider;

    [Header("Referências do Jogo (Cena)")]
    public Transform objetoAgua;

    [Header("Alturas da Água (Posição Y)")]
    public float[] alturasPorNivel = new float[5];
    public float velocidadeSubida = 2f;

    void Start()
    {
        if (barraAguaSlider != null)
        {
            barraAguaSlider.maxValue = nivelMaximo;
            barraAguaSlider.value = nivelAtual;
        }
    }

    void Update()
    {
        SubirAguaVisualmente();
        EncherComBuracos();
    }

    void SubirAguaVisualmente()
    {
        if (objetoAgua != null && nivelAtual < alturasPorNivel.Length)
        {
            float alturaAlvo = alturasPorNivel[nivelAtual];

            Vector3 novaPosicao = new Vector3(
                objetoAgua.position.x,
                alturaAlvo,
                objetoAgua.position.z
            );

            objetoAgua.position = Vector3.MoveTowards(
                objetoAgua.position,
                novaPosicao,
                velocidadeSubida * Time.deltaTime
            );
        }
    }

    void EncherComBuracos()
    {
        if (buracosAbertos <= 0)
            return;

        if (nivelAtual >= nivelMaximo)
            return;

        contadorAgua += Time.deltaTime * buracosAbertos;

        if (contadorAgua >= tempoParaSubirNivel)
        {
            contadorAgua = 0f;
            AumentarNivelAgua();
        }
    }

    public void AdicionarBuraco()
    {
        buracosAbertos++;
        Debug.Log("Buracos abertos: " + buracosAbertos);
    }

    public void RemoverBuraco()
    {
        buracosAbertos--;

        if (buracosAbertos < 0)
            buracosAbertos = 0;
    }

    public void AumentarNivelAgua()
    {
        if (nivelAtual < nivelMaximo)
        {
            nivelAtual++;
            AtualizarHUD();
        }

        if (nivelAtual >= nivelMaximo)
        {
            HUDManager hud = FindFirstObjectByType<HUDManager>();

            if (hud != null)
            {
                hud.Derrota();
            }
        }
    }

    private void AtualizarHUD()
    {
        if (barraAguaSlider != null)
        {
            barraAguaSlider.value = nivelAtual;
        }
    }
    public int PegarNivelAtual()
{
    return nivelAtual;
}

public void DiminuirNivelAgua()
{
    if (nivelAtual > 0)
    {
        nivelAtual--;
        AtualizarHUD();
        Debug.Log("Nível da água diminuiu: " + nivelAtual);
    }
}
}
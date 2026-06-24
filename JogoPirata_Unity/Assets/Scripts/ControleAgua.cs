using UnityEngine;
using UnityEngine.UI;

public class ControleAgua : MonoBehaviour
{
    [Header("Configurações de Nível")]
    public int nivelAtual = 0;
    public int nivelMaximo = 4;

    [Header("Referências da Interface (HUD)")]
    public Slider barraAguaSlider; 

    [Header("Referências do Jogo (Cena)")]
    public Transform objetoAgua; 

    [Header("Alturas da Água (Posição Y)")]
    [Tooltip("Coloque aqui a posição Y da água para cada nível (do 0 ao 4)")]
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
        if (objetoAgua != null && nivelAtual < alturasPorNivel.Length)
        {
            float alturaAlvo = alturasPorNivel[nivelAtual];
            Vector3 novaPosicao = new Vector3(objetoAgua.position.x, alturaAlvo, objetoAgua.position.z);
            
            objetoAgua.position = Vector3.MoveTowards(objetoAgua.position, novaPosicao, velocidadeSubida * Time.deltaTime);
        }

        // APENAS PARA TESTE: Aperte a tecla ESPAÇO para simular o barco enchendo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AumentarNivelAgua();
        }
    }

    public void AumentarNivelAgua()
    {
        if (nivelAtual < nivelMaximo)
        {
            nivelAtual++;
            AtualizarHUD();
        }
    }

    public void DiminuirNivelAgua()
    {
        if (nivelAtual > 0)
        {
            nivelAtual--;
            AtualizarHUD();
        }
    }

    private void AtualizarHUD()
    {
        if (barraAguaSlider != null)
        {
            barraAguaSlider.value = nivelAtual;
        }
    }
}
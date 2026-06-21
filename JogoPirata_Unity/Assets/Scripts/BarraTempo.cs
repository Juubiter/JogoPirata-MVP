using UnityEngine;
using UnityEngine.UI;

public class BarraTempo : MonoBehaviour
{
    public Slider sliderTempo;
    public float tempoDaFase = 30f;

    private float tempoAtual;

    void Start()
    {
        tempoAtual = tempoDaFase;

        sliderTempo.maxValue = tempoDaFase;
        sliderTempo.value = tempoDaFase;
    }

    void Update()
    {
        tempoAtual -= Time.deltaTime;

        sliderTempo.value = tempoAtual;

        if (tempoAtual <= 0)
        {
            tempoAtual = 0;
        }
    }
}
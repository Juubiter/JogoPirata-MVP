using UnityEngine;
using System.Collections;

public class NPCMovimentoNavio : MonoBehaviour
{
    public float velocidadeAndar = 2f;
    public float velocidadeEscada = 1.5f;

    public float tempoMinimoParado = 1f;
    public float tempoMaximoParado = 3f;
    public float chanceTrocarAndar = 0.3f;

    public Transform limiteEsquerda;
    public Transform limiteDireita;

    public Transform escadaEsquerda;
    public Transform escadaDireita;

    public Transform pontoConves;
    public Transform pontoMeio;
    public Transform pontoBaixo;

    private int andarAtual = 1;
    private float destinoX;
    private bool ocupado = false;

    void Start()
    {
        FixarYNoAndarAtual();
        EscolherDestinoHorizontal();
    }

    void Update()
    {
        if (ocupado)
            return;

        AndarHorizontal();
    }

    void AndarHorizontal()
    {
        Vector3 pos = transform.localPosition;

        pos.x = Mathf.MoveTowards(pos.x, destinoX, velocidadeAndar * Time.deltaTime);
        pos.y = PegarYDoAndar(andarAtual);

        transform.localPosition = pos;

        if (Mathf.Abs(transform.localPosition.x - destinoX) < 0.05f)
        {
            StartCoroutine(EsperarEEscolher());
        }
    }

    IEnumerator EsperarEEscolher()
    {
        ocupado = true;

        yield return new WaitForSeconds(Random.Range(tempoMinimoParado, tempoMaximoParado));

        if (Random.value < chanceTrocarAndar)
            yield return StartCoroutine(TrocarAndar());
        else
            EscolherDestinoHorizontal();

        ocupado = false;
    }

    void EscolherDestinoHorizontal()
    {
        destinoX = Random.Range(
            limiteEsquerda.localPosition.x,
            limiteDireita.localPosition.x
        );
    }

    IEnumerator TrocarAndar()
    {
        int novoAndar = andarAtual;

        if (andarAtual == 0)
            novoAndar = 1;
        else if (andarAtual == 1)
            novoAndar = Random.value < 0.5f ? 0 : 2;
        else if (andarAtual == 2)
            novoAndar = 1;

        Transform escada = null;

        if ((andarAtual == 0 && novoAndar == 1) || (andarAtual == 1 && novoAndar == 0))
            escada = escadaDireita;

        if ((andarAtual == 1 && novoAndar == 2) || (andarAtual == 2 && novoAndar == 1))
            escada = escadaEsquerda;

        if (escada == null)
            yield break;

        yield return StartCoroutine(IrAteX(escada.localPosition.x));

        float novoY = PegarYDoAndar(novoAndar);

        yield return StartCoroutine(SubirOuDescer(novoY));

        andarAtual = novoAndar;
        FixarYNoAndarAtual();
        EscolherDestinoHorizontal();
    }

    IEnumerator IrAteX(float xAlvo)
    {
        while (Mathf.Abs(transform.localPosition.x - xAlvo) > 0.05f)
        {
            Vector3 pos = transform.localPosition;
            pos.x = Mathf.MoveTowards(pos.x, xAlvo, velocidadeAndar * Time.deltaTime);
            pos.y = PegarYDoAndar(andarAtual);
            transform.localPosition = pos;

            yield return null;
        }
    }

    IEnumerator SubirOuDescer(float yAlvo)
    {
        while (Mathf.Abs(transform.localPosition.y - yAlvo) > 0.05f)
        {
            Vector3 pos = transform.localPosition;
            pos.y = Mathf.MoveTowards(pos.y, yAlvo, velocidadeEscada * Time.deltaTime);
            transform.localPosition = pos;

            yield return null;
        }
    }

    void FixarYNoAndarAtual()
    {
        Vector3 pos = transform.localPosition;
        pos.y = PegarYDoAndar(andarAtual);
        transform.localPosition = pos;
    }

    float PegarYDoAndar(int andar)
    {
        if (andar == 2)
            return pontoConves.localPosition.y;

        if (andar == 1)
            return pontoMeio.localPosition.y;

        return pontoBaixo.localPosition.y;
    }
}
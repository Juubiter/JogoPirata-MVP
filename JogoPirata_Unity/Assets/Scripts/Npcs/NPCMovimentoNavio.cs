using UnityEngine;
using System.Collections;

public class NPCMovimentoNavio : MonoBehaviour
{
    public enum Tarefa { Aleatorio, IndoApagarFogo, IndoTamparBuraco, TirandoAgua, BuscandoMunicao, IndoAtirar }

    [Header("Estado Atual")]
    public Tarefa tarefaAtual = Tarefa.Aleatorio;

    public float velocidadeAndar = 2f;
    public float velocidadeEscada = 1.5f;

    public float tempoMinimoParado = 1f;
    public float tempoMaximoParado = 3f;
    public float chanceTrocarAndar = 0.3f;

    private Animator meuAnimator;
    private SpriteRenderer meuSpriteRenderer;
    private Vector3 posicaoAnterior;
    private float limiteMovimento = 0.001f;
    private int estadoAnimacaoAtual = 0;

    [Header("Itens na Mão")]
    public GameObject iconeMunicaoMao;
    public GameObject iconeBaldeCheio;

    [Header("Limites do Baixo")]
    public Transform limiteEsquerdaBaixo;
    public Transform limiteDireitaBaixo;

    [Header("Limites do Meio")]
    public Transform limiteEsquerdaMeio;
    public Transform limiteDireitaMeio;

    [Header("Limites do Convés")]
    public Transform limiteEsquerdaConves;
    public Transform limiteDireitaConves;

    [Header("Limites do Leme")]
    public Transform limiteEsquerdaLeme;
    public Transform limiteDireitaLeme;

    [Header("Escadas")]
    public Transform escadaEsquerda;
    public Transform escadaDireita;
    public Transform escadaLeme;

    [Header("Pontos de Altura")]
    public Transform pontoConves;
    public Transform pontoMeio;
    public Transform pontoBaixo;
    public Transform pontoLeme;

    private int andarAtual;
    private float destinoX;
    private bool ocupado = false;

    void Start()
    {
        meuAnimator = GetComponent<Animator>();
        meuSpriteRenderer = GetComponent<SpriteRenderer>();
        posicaoAnterior = transform.localPosition;

        andarAtual = Random.Range(0, 4);

        EscolherDestinoHorizontal();

        Vector3 pos = transform.localPosition;
        pos.x = destinoX;
        transform.localPosition = pos;

        FixarYNoAndarAtual();
        EscolherDestinoHorizontal();
    }

    void Update()
    {
        if (tarefaAtual == Tarefa.Aleatorio)
        {
            if (ocupado) return;
            AndarHorizontal();
        }
    }

    void LateUpdate()
    {
        float distanciaMovimentada = Vector3.Distance(transform.localPosition, posicaoAnterior);
        bool estaSeMovendo = distanciaMovimentada > limiteMovimento;

        if (!estaSeMovendo)
            estadoAnimacaoAtual = 0;
        else if (Mathf.Abs(transform.localPosition.y - posicaoAnterior.y) > 0.01f)
            estadoAnimacaoAtual = 2;
        else
            estadoAnimacaoAtual = 1;

        if (meuAnimator != null)
            meuAnimator.SetInteger("EstadoMovimento", estadoAnimacaoAtual);

        if (estaSeMovendo && meuSpriteRenderer != null)
        {
            if (transform.localPosition.x > posicaoAnterior.x)
                meuSpriteRenderer.flipX = false;
            else if (transform.localPosition.x < posicaoAnterior.x)
                meuSpriteRenderer.flipX = true;
        }

        posicaoAnterior = transform.localPosition;
    }

    public void Comando_ApagarFogo(Transform alvoFogo, int andarDoFogo)
    {
        PrepararNovaTarefa(Tarefa.IndoApagarFogo);
        StartCoroutine(Rotina_ExecutarTarefa(alvoFogo, andarDoFogo, "ApagarFogo"));
    }

    public void Comando_TamparBuraco(Transform alvoBuraco, int andarDoBuraco)
    {
        PrepararNovaTarefa(Tarefa.IndoTamparBuraco);
        StartCoroutine(Rotina_ExecutarTarefa(alvoBuraco, andarDoBuraco, "TamparBuraco"));
    }

    public void Comando_TirarAgua(Transform localBeirada, int andarDaBeirada)
    {
        PrepararNovaTarefa(Tarefa.TirandoAgua);
        StartCoroutine(Rotina_ExecutarTarefa(localBeirada, andarDaBeirada, "TirarAgua"));
    }

    public void Comando_AtirarCanhao(Transform localMunicao, int andarMunicao, Transform localCanhao, int andarCanhao)
    {
        PrepararNovaTarefa(Tarefa.BuscandoMunicao);
        StartCoroutine(Rotina_Canhao(localMunicao, andarMunicao, localCanhao, andarCanhao));
    }

    private void PrepararNovaTarefa(Tarefa novaTarefa)
    {
        StopAllCoroutines();

        if (iconeMunicaoMao != null) iconeMunicaoMao.SetActive(false);
        if (iconeBaldeCheio != null) iconeBaldeCheio.SetActive(false);

        tarefaAtual = novaTarefa;
        ocupado = true;
    }

    IEnumerator Rotina_ExecutarTarefa(Transform alvo, int andarDestino, string nomeAcao)
    {
        if (alvo == null)
        {
            FinalizarTarefa();
            yield break;
        }

        yield return StartCoroutine(NavegarParaAndarEspecifico(andarDestino));

        if (alvo == null)
        {
            FinalizarTarefa();
            yield break;
        }

        yield return StartCoroutine(IrAteX(alvo.localPosition.x));

        Debug.Log("NPC CHEGOU! Executando ação: " + nomeAcao);
if (nomeAcao == "ApagarFogo")
{
    yield return StartCoroutine(ApagarFogoAosPoucos(alvo));
}
else if (nomeAcao == "TamparBuraco")
{
    yield return StartCoroutine(TamparBuracoAosPoucos(alvo));
}
else
{
    yield return new WaitForSeconds(2f);
}

        FinalizarTarefa();
    }


    IEnumerator ApagarFogoAosPoucos(Transform fogo)
    {
        if (fogo == null)
            yield break;

        Debug.Log("COMEÇOU A APAGAR O FOGO: " + fogo.name);

        yield return new WaitForSeconds(1.5f);

        if (fogo != null)
        {
            Debug.Log("DESTRUINDO FOGO: " + fogo.name);
            Destroy(fogo.gameObject);
        }
    }
 IEnumerator TamparBuracoAosPoucos(Transform buraco)
{
    if (buraco == null)
        yield break;

    Debug.Log("COMEÇOU A TAMPAR O BURACO: " + buraco.name);

    BuracoAgua scriptBuraco = buraco.GetComponent<BuracoAgua>();

    if (scriptBuraco != null)
    {
        scriptBuraco.Tampar();
        yield return new WaitForSeconds(0.2f);
    }
    else
    {
        Debug.LogWarning("O buraco não possui o script BuracoAgua!");
    }
}
    private void FinalizarTarefa()
    {
        tarefaAtual = Tarefa.Aleatorio;
        ocupado = false;
        EscolherDestinoHorizontal();
    }

    IEnumerator Rotina_Canhao(Transform municao, int andarMunicao, Transform canhao, int andarCanhao)
    {
        yield return StartCoroutine(NavegarParaAndarEspecifico(andarMunicao));
        yield return StartCoroutine(IrAteX(municao.localPosition.x));

        yield return new WaitForSeconds(1.5f);

        if (iconeMunicaoMao != null)
            iconeMunicaoMao.SetActive(true);

        tarefaAtual = Tarefa.IndoAtirar;

        yield return StartCoroutine(NavegarParaAndarEspecifico(andarCanhao));
        yield return StartCoroutine(IrAteX(canhao.localPosition.x));

        if (iconeMunicaoMao != null)
            iconeMunicaoMao.SetActive(false);

        yield return new WaitForSeconds(3f);

        Debug.Log("BUM! Atirou o Canhão!");

        FinalizarTarefa();
    }

    public void Comando_TirarAgua(int andarDaAgua, Transform localBeiradaConves)
    {
        PrepararNovaTarefa(Tarefa.TirandoAgua);
        StartCoroutine(Rotina_CicloAgua(andarDaAgua, localBeiradaConves));
    }

    IEnumerator Rotina_CicloAgua(int andarDaAgua, Transform localBeiradaConves)
    {
        yield return StartCoroutine(NavegarParaAndarEspecifico(andarDaAgua));

        yield return new WaitForSeconds(1.5f);

        if (iconeBaldeCheio != null)
            iconeBaldeCheio.SetActive(true);

        yield return StartCoroutine(NavegarParaAndarEspecifico(2));
        yield return StartCoroutine(IrAteX(localBeiradaConves.localPosition.x));

        yield return new WaitForSeconds(1f);

        if (iconeBaldeCheio != null)
            iconeBaldeCheio.SetActive(false);

        Debug.Log("Jogou água no mar! Diminuindo a barra...");

        FinalizarTarefa();
    }

    IEnumerator NavegarParaAndarEspecifico(int andarDestino)
    {
        while (andarAtual != andarDestino)
        {
            int proximoAndar = andarAtual < andarDestino ? andarAtual + 1 : andarAtual - 1;
            Transform escadaUsada = null;

            if ((andarAtual == 0 && proximoAndar == 1) || (andarAtual == 1 && proximoAndar == 0))
                escadaUsada = escadaDireita;
            else if ((andarAtual == 1 && proximoAndar == 2) || (andarAtual == 2 && proximoAndar == 1))
                escadaUsada = escadaEsquerda;
            else if ((andarAtual == 2 && proximoAndar == 3) || (andarAtual == 3 && proximoAndar == 2))
                escadaUsada = escadaLeme;

            if (escadaUsada != null)
            {
                float xInicio = escadaUsada.localPosition.x;
                float xFim = escadaUsada.localPosition.x;

                if (escadaUsada == escadaLeme)
                {
                    xInicio = andarAtual == 2 ? escadaLeme.localPosition.x : pontoLeme.localPosition.x;
                    xFim = proximoAndar == 2 ? escadaLeme.localPosition.x : pontoLeme.localPosition.x;
                }

                yield return StartCoroutine(IrAteX(xInicio));
                yield return StartCoroutine(MoverNaEscada(xFim, PegarYDoAndar(proximoAndar)));

                andarAtual = proximoAndar;
                FixarYNoAndarAtual();
            }
            else
            {
                yield break;
            }
        }
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
            yield return StartCoroutine(TrocarAndarAleatorio());
        else
            EscolherDestinoHorizontal();

        ocupado = false;
    }

    IEnumerator TrocarAndarAleatorio()
    {
        int novoAndar = andarAtual;

        if (andarAtual == 0)
            novoAndar = 1;
        else if (andarAtual == 1)
            novoAndar = Random.value < 0.5f ? 0 : 2;
        else if (andarAtual == 2)
            novoAndar = Random.value < 0.5f ? 1 : 3;
        else if (andarAtual == 3)
            novoAndar = 2;

        yield return StartCoroutine(NavegarParaAndarEspecifico(novoAndar));
        EscolherDestinoHorizontal();
    }

    void EscolherDestinoHorizontal()
    {
        float minX = 0f;
        float maxX = 0f;

        if (andarAtual == 3)
        {
            minX = limiteEsquerdaLeme.localPosition.x;
            maxX = limiteDireitaLeme.localPosition.x;
        }
        else if (andarAtual == 2)
        {
            minX = limiteEsquerdaConves.localPosition.x;
            maxX = limiteDireitaConves.localPosition.x;
        }
        else if (andarAtual == 1)
        {
            minX = limiteEsquerdaMeio.localPosition.x;
            maxX = limiteDireitaMeio.localPosition.x;
        }
        else
        {
            minX = limiteEsquerdaBaixo.localPosition.x;
            maxX = limiteDireitaBaixo.localPosition.x;
        }

        destinoX = Random.Range(minX, maxX);
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

    IEnumerator MoverNaEscada(float xAlvo, float yAlvo)
    {
        Vector3 destino = new Vector3(xAlvo, yAlvo, transform.localPosition.z);

        while (Vector3.Distance(transform.localPosition, destino) > 0.05f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                destino,
                velocidadeEscada * Time.deltaTime
            );

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
        if (andar == 3)
            return pontoLeme.localPosition.y;

        if (andar == 2)
            return pontoConves.localPosition.y;

        if (andar == 1)
            return pontoMeio.localPosition.y;

        return pontoBaixo.localPosition.y;
    }
}
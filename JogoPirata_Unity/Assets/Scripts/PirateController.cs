using UnityEngine;

public class PirateController : MonoBehaviour
{
    public GameObject highlight;
    private NPCMovimentoNavio scriptMovimento;

    void Start()
    {
        scriptMovimento = GetComponent<NPCMovimentoNavio>();
    }

    public void Selecionar()
    {
        if (highlight != null)
            highlight.SetActive(true);

        if (GerenciadorMenuAcoes.Instancia != null)
            GerenciadorMenuAcoes.Instancia.pirataSelecionado = this;
    }

    public void Deselecionar()
    {
        if (highlight != null)
            highlight.SetActive(false);

        if (GerenciadorMenuAcoes.Instancia != null &&
            GerenciadorMenuAcoes.Instancia.pirataSelecionado == this)
        {
            GerenciadorMenuAcoes.Instancia.pirataSelecionado = null;
        }
    }

    public void ReceberOrdem(OrdemPirata ordem)
    {
        Debug.Log(gameObject.name + " recebeu ordem: " + ordem);

        if (scriptMovimento == null)
        {
            Debug.LogWarning("NPCMovimentoNavio não encontrado no pirata!");
            return;
        }

        switch (ordem)
        {
            case OrdemPirata.ApagarFogo:
                BuscarEApagarFogo();
                break;

            case OrdemPirata.TamparBuraco:
                BuscarETamparBuraco();
                break;

            case OrdemPirata.TirarAgua:
                IrTirarAgua();
                break;

            case OrdemPirata.AtirarCanhao:
                BuscarMunicaoEAtirar();
                break;
        }
    }
private void BuscarEApagarFogo()
{
    Transform alvo = EncontrarFogoMaisProximoDisponivel();

    if (alvo == null)
    {
        Debug.LogWarning("Nenhum fogo disponível encontrado!");
        return;
    }

    AlvoReservado AlvoReservado = alvo.GetComponent<AlvoReservado>();

    if (AlvoReservado == null)
        AlvoReservado = alvo.gameObject.AddComponent<AlvoReservado>();

    if (!AlvoReservado.Reservar(this))
    {
        Debug.LogWarning("Esse fogo já foi reservado por outro NPC!");
        return;
    }

    int andarDoFogo = DescobrirAndarPelaAltura(alvo.position.y);

    Debug.Log("FOGO RESERVADO POR: " + gameObject.name);

    scriptMovimento.Comando_ApagarFogo(alvo, andarDoFogo);
}
    private Transform EncontrarFogoMaisProximoDisponivel()
    {
        GameObject[] fogos = GameObject.FindGameObjectsWithTag("Fogo");

        if (fogos.Length == 0)
        {
            Debug.LogWarning("Não encontrei nenhum objeto com a tag Fogo.");
            return null;
        }

        Transform maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (GameObject fogoObj in fogos)
        {
            if (fogoObj == null)
                continue;

            AlvoReservado AlvoReservado = fogoObj.GetComponent<AlvoReservado>();

            if (AlvoReservado == null)
                AlvoReservado = fogoObj.AddComponent<AlvoReservado>();

            if (AlvoReservado.EstaReservado())
                continue;

            float distancia = Vector3.Distance(transform.position, fogoObj.transform.position);

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = fogoObj.transform;
            }
        }

        return maisProximo;
    }
    private Transform EncontrarAlvoDisponivelMaisProximo(string tagDoObjeto)
{
    GameObject[] objetos = GameObject.FindGameObjectsWithTag(tagDoObjeto);

    if (objetos.Length == 0)
    {
        Debug.LogWarning("Não encontrei nenhum objeto com a tag: " + tagDoObjeto);
        return null;
    }

    Transform maisProximo = null;
    AlvoReservado reservaMaisProxima = null;
    float menorDistancia = Mathf.Infinity;

    foreach (GameObject obj in objetos)
    {
        if (obj == null)
            continue;

        AlvoReservado reserva = obj.GetComponent<AlvoReservado>();

        if (reserva == null)
            reserva = obj.AddComponent<AlvoReservado>();

        if (reserva.EstaReservado())
            continue;

        float distancia = Vector3.Distance(transform.position, obj.transform.position);

        if (distancia < menorDistancia)
        {
            menorDistancia = distancia;
            maisProximo = obj.transform;
            reservaMaisProxima = reserva;
        }
    }

    if (maisProximo != null && reservaMaisProxima != null)
    {
        if (!reservaMaisProxima.Reservar(this))
            return null;
    }

    return maisProximo;
}

    private void BuscarETamparBuraco()
{
    Transform alvo = EncontrarAlvoDisponivelMaisProximo("BuracoAgua");

    if (alvo == null)
    {
        Debug.LogWarning("Nenhum buraco de água disponível encontrado!");
        return;
    }

    int andarDoBuraco = DescobrirAndarPelaAltura(alvo.position.y);
    scriptMovimento.Comando_TamparBuraco(alvo, andarDoBuraco);
}

    private void IrTirarAgua()
    {
        GameObject aguaObj = GameObject.Find("AguaInundacao");
        Transform beirada = EncontrarMaisProximo("PontoAgua");

        if (aguaObj != null && beirada != null)
        {
            float alturaTopoAgua = aguaObj.transform.position.y + (aguaObj.transform.localScale.y / 2f);
            int andarDaAgua = DescobrirAndarPelaAltura(alturaTopoAgua);

            scriptMovimento.Comando_TirarAgua(andarDaAgua, beirada);
        }
        else
        {
            Debug.LogWarning("Não encontrei AguaInundacao ou PontoAgua!");
        }
    }

    private void BuscarMunicaoEAtirar()
    {
        Transform municao = EncontrarMaisProximo("Municao");
        Transform canhao = EncontrarMaisProximo("Canhao");

        if (municao != null && canhao != null)
        {
            int andarMunicao = DescobrirAndarPelaAltura(municao.position.y);
            int andarCanhao = DescobrirAndarPelaAltura(canhao.position.y);

            scriptMovimento.Comando_AtirarCanhao(municao, andarMunicao, canhao, andarCanhao);
        }
        else
        {
            Debug.LogWarning("Não encontrei Municao ou Canhao!");
        }
    }

    private Transform EncontrarMaisProximo(string tagDoObjeto)
    {
        GameObject[] todosOsObjetos = GameObject.FindGameObjectsWithTag(tagDoObjeto);

        if (todosOsObjetos.Length == 0)
        {
            Debug.LogWarning("Não encontrei nenhum objeto com a tag: " + tagDoObjeto);
            return null;
        }

        Transform maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (GameObject obj in todosOsObjetos)
        {
            float distancia = Vector3.Distance(transform.position, obj.transform.position);

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = obj.transform;
            }
        }

        return maisProximo;
    }

    private int DescobrirAndarPelaAltura(float alturaY)
    {
        if (scriptMovimento == null)
            return 0;

        if (alturaY > scriptMovimento.pontoConves.position.y - 0.5f)
            return 2;

        if (alturaY > scriptMovimento.pontoMeio.position.y - 0.5f)
            return 1;

        return 0;
    }
}
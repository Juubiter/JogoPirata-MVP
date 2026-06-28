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
        highlight.SetActive(true);

        if (GerenciadorMenuAcoes.Instancia != null)
        {
            GerenciadorMenuAcoes.Instancia.pirataSelecionado = this;
        }
    }

    public void Deselecionar()
    {
        highlight.SetActive(false);

        if (GerenciadorMenuAcoes.Instancia != null && GerenciadorMenuAcoes.Instancia.pirataSelecionado == this)
        {
            GerenciadorMenuAcoes.Instancia.pirataSelecionado = null;
        }
    }

    public void ReceberOrdem(OrdemPirata ordem)
    {
        Debug.Log(gameObject.name + " recebeu ordem: " + ordem);

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

    FogoReservado fogo = alvo.GetComponent<FogoReservado>();
    if (fogo != null)
    {
        fogo.Reservar(this);
    }

    Transform pontoApagar = alvo.Find("PontoApagar");
    if (pontoApagar == null)
    {
        pontoApagar = alvo;
    }

    int andarDoFogo = DescobrirAndarPelaAlturaGlobal(pontoApagar.position.y);

    scriptMovimento.Comando_ApagarFogo(alvo, andarDoFogo);
}
private int DescobrirAndarPelaAlturaGlobal(float alturaYGlobal)
{
    float yConves = scriptMovimento.pontoConves.position.y;
    float yMeio = scriptMovimento.pontoMeio.position.y;

    if (alturaYGlobal > yConves - 0.5f) return 2;
    if (alturaYGlobal > yMeio - 0.5f) return 1;
    return 0;
}
    private Transform EncontrarFogoMaisProximoDisponivel()
    {
        GameObject[] fogos = GameObject.FindGameObjectsWithTag("Fogo");

        if (fogos.Length == 0)
        {
            Debug.LogWarning("Não encontrei nenhum objeto com a tag: Fogo");
            return null;
        }

        Transform maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (GameObject fogoObj in fogos)
        {
            FogoReservado fogo = fogoObj.GetComponent<FogoReservado>();

            if (fogo == null)
            {
                fogo = fogoObj.AddComponent<FogoReservado>();
            }

            if (fogo.EstaReservado())
            {
                continue;
            }

            float distancia = Vector3.Distance(transform.position, fogoObj.transform.position);

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = fogoObj.transform;
            }
        }

        return maisProximo;
    }

    private void BuscarETamparBuraco()
    {
        Transform alvo = EncontrarMaisProximo("Buraco");

        if (alvo != null)
        {
            scriptMovimento.Comando_TamparBuraco(alvo, DescobrirAndarPelaAltura(alvo.position.y));
        }
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
            Debug.LogWarning("Não encontrei a AguaInundacao ou o PontoAgua!");
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
        if (alturaY > scriptMovimento.pontoConves.position.y - 0.5f) return 2;
        if (alturaY > scriptMovimento.pontoMeio.position.y - 0.5f) return 1;
        return 0;
    }
}
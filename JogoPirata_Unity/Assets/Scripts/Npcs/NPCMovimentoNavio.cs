using UnityEngine;
using System.Collections;

public class NPCMovimentoNavio : MonoBehaviour
{
    public float velocidadeAndar = 2f;
    public float velocidadeEscada = 1.5f;

    public float tempoMinimoParado = 1f;
    public float tempoMaximoParado = 3f;
    public float chanceTrocarAndar = 0.3f;

    // --- NOVOS LIMITES INDIVIDUAIS POR ANDAR ---
    [Header("Limites do Baixo (Andar 0)")]
    public Transform limiteEsquerdaBaixo;
    public Transform limiteDireitaBaixo;

    [Header("Limites do Meio (Andar 1)")]
    public Transform limiteEsquerdaMeio;
    public Transform limiteDireitaMeio;

    [Header("Limites do Convés (Andar 2)")]
    public Transform limiteEsquerdaConves;
    public Transform limiteDireitaConves;

    [Header("Limites do Leme (Andar 3)")]
    public Transform limiteEsquerdaLeme;
    public Transform limiteDireitaLeme;

    [Header("Escadas")]
    public Transform escadaEsquerda;
    public Transform escadaDireita;
    public Transform escadaLeme; 

    [Header("Pontos de Altura (Y)")]
    public Transform pontoConves;
    public Transform pontoMeio;
    public Transform pontoBaixo;
    public Transform pontoLeme; 

    private int andarAtual;
    private float destinoX;
    private bool ocupado = false;

    void Start()
    {
        // 1. Sorteia um andar aleatório entre 0 e 3 
        // (Random.Range com int exclui o número final, por isso 4)
        andarAtual = Random.Range(0, 4);

        // 2. Sorteia um X válido para o andar que acabou de ser escolhido
        EscolherDestinoHorizontal(); 

        // 3. Posiciona o NPC exatamente nesse X para ele não nascer flutuando fora do barco
        Vector3 pos = transform.localPosition;
        pos.x = destinoX; 
        transform.localPosition = pos;

        // 4. Arruma a altura (Y) de acordo com o andar
        FixarYNoAndarAtual();

        // 5. Sorteia o primeiro destino real de caminhada dele e vida que segue!
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

    // ALTERAÇÃO: Agora ele sorteia o destino respeitando o limite do andar que ele está!
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
        else if (andarAtual == 0)
        {
            minX = limiteEsquerdaBaixo.localPosition.x;
            maxX = limiteDireitaBaixo.localPosition.x;
        }

        destinoX = Random.Range(minX, maxX);
    }

    IEnumerator TrocarAndar()
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

        Transform escada = null;

        if ((andarAtual == 0 && novoAndar == 1) || (andarAtual == 1 && novoAndar == 0))
            escada = escadaDireita;

        if ((andarAtual == 1 && novoAndar == 2) || (andarAtual == 2 && novoAndar == 1))
            escada = escadaEsquerda;

        if ((andarAtual == 2 && novoAndar == 3) || (andarAtual == 3 && novoAndar == 2))
            escada = escadaLeme;

        if (escada == null)
            yield break;

        float xInicioEscada;
        float xFimEscada;

        if (escada == escadaLeme)
        {
            if (andarAtual == 2) 
            {
                xInicioEscada = escadaLeme.localPosition.x; 
                xFimEscada = pontoLeme.localPosition.x;     
            }
            else 
            {
                xInicioEscada = pontoLeme.localPosition.x;  
                xFimEscada = escadaLeme.localPosition.x;    
            }
        }
        else
        {
            xInicioEscada = escada.localPosition.x;
            xFimEscada = escada.localPosition.x;
        }

        yield return StartCoroutine(IrAteX(xInicioEscada));

        float novoY = PegarYDoAndar(novoAndar);

        yield return StartCoroutine(MoverNaEscada(xFimEscada, novoY));

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

    IEnumerator MoverNaEscada(float xAlvo, float yAlvo)
    {
        Vector3 destino = new Vector3(xAlvo, yAlvo, transform.localPosition.z);

        while (Vector3.Distance(transform.localPosition, destino) > 0.05f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destino, velocidadeEscada * Time.deltaTime);
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
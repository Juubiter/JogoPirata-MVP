using UnityEngine;

public class InimigoAtirador : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject balaPrefab;       // O tiro do zepelim
    public Transform[] pontosDeDisparo; // Lista de canhões
    public Transform alvo;              // O seu Barco Principal
    
    [Header("Atributos de Tiro")]
    public float forcaDoTiro = 15f;     // Velocidade da bala
    public float tempoEntreTiros = 3f;  // Atira a cada 3 segundos depois que começar
    
    [Header("Atraso Inicial")]
    [Tooltip("Quantos segundos o inimigo espera antes de dar o primeiro tiro.")]
    public float tempoParaComecar = 5f; // <--- O tempo que você pediu aqui!
    
    private float tempoProximoTiro;

    void Start()
    {
        // Assim que o Zepelim aparece na tela, nós adicionamos o seu tempo de espera
        // Isso obriga o Update a aguardar esses "X" segundos antes de atirar a primeira vez
        tempoProximoTiro = Time.time + tempoParaComecar;
    }

    void Update()
    {
        // Verifica se o relógio do jogo já passou do tempo de espera
        if (Time.time >= tempoProximoTiro)
        {
            Atirar();
            
            // Depois do primeiro tiro, ele passa a respeitar apenas o "tempoEntreTiros" normal
            tempoProximoTiro = Time.time + tempoEntreTiros;
        }
    }

    void Atirar()
    {
        if (alvo == null || pontosDeDisparo.Length == 0) return;

        int indexSorteado = Random.Range(0, pontosDeDisparo.Length);
        Transform pontoEscolhido = pontosDeDisparo[indexSorteado];

        Vector2 direcao = (alvo.position - pontoEscolhido.position).normalized;

        GameObject novaBala = Instantiate(balaPrefab, pontoEscolhido.position, Quaternion.identity);

        Rigidbody2D rb = novaBala.GetComponent<Rigidbody2D>();
        rb.AddForce(direcao * forcaDoTiro, ForceMode2D.Impulse);

        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        novaBala.transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
}
using UnityEngine;

public class ObjetoInterativo : MonoBehaviour
{
    public Ordem ordem;


    public void ReceberOrdem(PirateController pirata)
    {
        Debug.Log(
            pirata.name +
            " recebeu ordem: " +
            ordem
        );
    }
}
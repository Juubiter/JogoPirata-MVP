using UnityEngine;

public class FogoReservado : MonoBehaviour
{
    private PirateController npcReservado;

    public void Reservar(PirateController npc)
    {
        npcReservado = npc;
    }

    public bool EstaReservado()
    {
        return npcReservado != null;
    }

    public void Liberar()
    {
        npcReservado = null;
    }
}
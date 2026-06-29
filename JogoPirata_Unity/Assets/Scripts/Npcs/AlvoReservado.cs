using UnityEngine;

public class AlvoReservado : MonoBehaviour
{
    private PirateController npcReservado;

    public bool Reservar(PirateController npc)
    {
        if (npcReservado != null)
            return false;

        npcReservado = npc;
        return true;
    }

    public void Liberar()
    {
        npcReservado = null;
    }

    public bool EstaReservado()
    {
        return npcReservado != null;
    }

    public PirateController QuemReservou()
    {
        return npcReservado;
    }
}
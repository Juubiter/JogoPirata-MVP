using UnityEngine;

public class FogoReservado : MonoBehaviour
{
    private PirateController npcReservado;

    public bool Reservar(PirateController npc)
    {
        if (npcReservado != null)
            return false;

        npcReservado = npc;
        return true;
    }

    public bool EstaReservado()
    {
        return npcReservado != null;
    }

    public bool ReservadoPor(PirateController npc)
    {
        return npcReservado == npc;
    }

    public void Liberar()
    {
        npcReservado = null;
    }
}
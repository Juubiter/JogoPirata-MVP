using UnityEngine;
using System.Collections;

public class BuracoAgua : MonoBehaviour
{
    [Header("Sprite que será trocado")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite buracoAberto;
    public Sprite buracoTampado;

    private bool foiTampado = false;

    public void Tampar()
    {
        if (foiTampado)
            return;

        foiTampado = true;

        StartCoroutine(RotinaTampar());
    }
IEnumerator RotinaTampar()
{
    if (spriteRenderer != null && buracoTampado != null)
        spriteRenderer.sprite = buracoTampado;

    ControleAgua controle = FindFirstObjectByType<ControleAgua>();

    if (controle != null)
        controle.RemoverBuraco();

    AlvoReservado reserva = GetComponent<AlvoReservado>();

    if (reserva != null)
        reserva.Liberar();

    yield return new WaitForSeconds(6f);

    Destroy(gameObject);
}
}
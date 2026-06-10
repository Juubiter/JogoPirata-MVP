using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            VerificarClique();
        }
    }

    void VerificarClique()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            PirateController pirata =
                hit.collider.GetComponent<PirateController>();

            if (pirata != null)
            {
                pirata.Selecionar();
            }
        }
    }
}
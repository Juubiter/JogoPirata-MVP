using UnityEngine;

public class BalaInimiga : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
}
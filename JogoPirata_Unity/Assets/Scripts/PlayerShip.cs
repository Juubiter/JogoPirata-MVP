using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [Header("Balanço")]
    [Min(0f), Tooltip("Altura do balanço em unidades de mundo.")]
    public float swayAmplitude = 0.15f;
    [Min(0f), Tooltip("Velocidade do balanço (ciclos por segundo).")]
    public float swayFrequency = 1.2f;

    private Vector2 basePosition;
    private float swayOffset;

    void Start()
    {
        basePosition = (Vector2)transform.root.position;
        swayOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swayFrequency + swayOffset) * swayAmplitude;
        Transform root = transform.root;
        root.position = new Vector3(
            basePosition.x,
            basePosition.y + sway,
            root.position.z
        );
    }
}

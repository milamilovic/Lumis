using UnityEngine;

public class SeedBob : MonoBehaviour
{
    private Vector3 startPos;
    private float speed = 2f;
    private float height = 0.08f;

    void Start() => startPos = transform.position;

    void Update()
    {
        transform.position = startPos +
            new Vector3(0, Mathf.Sin(Time.time * speed) * height, 0);
    }
}
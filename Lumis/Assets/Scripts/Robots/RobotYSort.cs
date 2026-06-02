using UnityEngine;

public class RobotYSort : MonoBehaviour
{
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sr == null) return;
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
    }
}
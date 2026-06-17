using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public float transparencyAmount = 0.5f;
    public float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private Transform player;

    private float targetAlpha = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null)
            return;

        // Fade
        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        sr.color = c;

        // Sorting
        sr.sortingOrder =
            player.position.y > transform.position.y
            ? 9999
            : -500;
    }

    public void SetTransparent(bool transparent)
    {
        targetAlpha = transparent ? transparencyAmount : 1f;
    }
}
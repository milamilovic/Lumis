using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public float transparencyAmount = 0.5f;
    public float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private Transform player;
    private float targetAlpha = 1f;

    private Collider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (player == null) return;

        float spriteHeight = sr.bounds.size.y;
        float spriteWidth = sr.bounds.size.x;

        float objectBase = transform.position.y - spriteHeight * 0.33f;

        bool playerIsBehind = player.position.y > objectBase && player.position.y < objectBase + spriteHeight * 1.5;
        bool xOverlap = Mathf.Abs(player.position.x - transform.position.x) < spriteWidth * 0.5f;

        targetAlpha = (xOverlap && playerIsBehind) ? transparencyAmount : 1f;

        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        sr.color = c;

        sr.sortingOrder = playerIsBehind ? 9999 : -500;
    }
}
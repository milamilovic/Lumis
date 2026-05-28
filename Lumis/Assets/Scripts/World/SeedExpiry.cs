using UnityEngine;

public class SeedExpiry : MonoBehaviour
{
    public float lifetime = 30f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // blink 5s before expiry
        if (timer > lifetime - 5f)
        {
            float blink = Mathf.Sin(Time.time * 10f);
            GetComponent<SpriteRenderer>().enabled = blink > 0f;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
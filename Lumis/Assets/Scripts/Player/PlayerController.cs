using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 input;
    private string facingDir = "down";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float x = 0f, y = 0f;

        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;

        input = new Vector2(x, y).normalized;

        UpdateFacing();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }

    void UpdateFacing()
    {
        if (input == Vector2.zero) return;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            facingDir = input.x > 0 ? "right" : "left";
        else
            facingDir = input.y < 0 ? "down" : "up";
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        string state;
        string dir;

        if (input != Vector2.zero)
        {
            // walking
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                dir = "side";
            else
                dir = input.y < 0 ? "front" : "back";

            state = "walking";
        }
        else
        {
            // idle
            if (facingDir == "right" || facingDir == "left")
                dir = "side";
            else if (facingDir == "up")
                dir = "back";
            else
                dir = "front";

            state = "idle";
        }

        sr.flipX = (facingDir == "left");
        anim.Play(state + "-" + dir);
    }
}
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

    [Header("Footsteps")]
    public AudioSource footstepLoopSource;
    public AudioClip indoorFootstepLoop;
    public float footstepPitch = 2f;
    public float fadeSpeed = 8f;

    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        SetupFootstepSource();
    }

    void SetupFootstepSource()
    {
        Debug.Log($"footstepLoopSource: {footstepLoopSource}, indoorFootstepLoop: {indoorFootstepLoop}");
        if (footstepLoopSource == null || indoorFootstepLoop == null) return;
        footstepLoopSource.clip = indoorFootstepLoop;
        footstepLoopSource.loop = true;
        footstepLoopSource.pitch = footstepPitch;
        footstepLoopSource.volume = 0f;
        footstepLoopSource.spatialBlend = 0f;
        footstepLoopSource.Play();
        Debug.Log($"Footstep source playing: {footstepLoopSource.isPlaying}, clip: {footstepLoopSource.clip}");
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
        HandleFootsteps();

        // y-sort when higher on screen is drawn behind
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
    }

    void HandleFootsteps()
    {
        if (footstepLoopSource == null) return;

        bool isWalking = input != Vector2.zero;
        bool isIndoors = playerHealth != null && playerHealth.isIndoors;
        float targetVolume = (isWalking && isIndoors) ? 8f : 0f;

        footstepLoopSource.volume = Mathf.MoveTowards(
            footstepLoopSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
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

    public void SetFacingDirection(string newDir)
    {
        facingDir = newDir;
        UpdateAnimation();
    }

    public void FaceForward() => SetFacingDirection("down");
}
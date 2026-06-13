using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
public enum RobotState { Idle, Wandering, Working, WalkingToTile }

public class Robot : MonoBehaviour
{
    public RobotDefinition definition { get; private set; }

    public RobotState state = RobotState.Idle;
    private List<Vector3Int> assignedTiles = new();
    private int currentTileIndex = 0;

    private SpriteRenderer sr;
    private Animator anim;
    private Rigidbody2D rb;

    [HideInInspector] public string seedToPlant;
    [HideInInspector] public GameObject plantPrefab;

    private Tilemap groundTilemap;
    private Tilemap dugTilemap;

    [Header("Wandering")]
    public float wanderRadius = 3f;
    public float wanderWaitMin = 2f;
    public float wanderWaitMax = 5f;

    private Coroutine wanderCoroutine;

    void Update()
    {
        if (rb == null) return;

        // Manual click detection
        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(
        UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            // check if click landed on this robot's non-trigger collider
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
            if (hit != null && hit.gameObject == gameObject && !hit.isTrigger)
            {
                if (state != RobotState.Working && state != RobotState.WalkingToTile)
                    TaskAssignmentManager.Instance?.SelectRobot(this);
            }
        }

        if (state == RobotState.Idle)
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null && anim.enabled)
            {
                anim.enabled = false;
            }
        }
        else
        {
            if (anim != null && !anim.enabled)
                anim.enabled = true;
        }
    }

    public void Initialize(RobotDefinition def)
    {
        definition = def;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (def.idleSprite != null) sr.sprite = def.idleSprite;
        if (def.animatorController != null) anim.runtimeAnimatorController = def.animatorController;

        groundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();
        dugTilemap = GameObject.Find("DugGround")?.GetComponent<Tilemap>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearDamping = 20f;

        StartWandering();
    }

    public void AssignTiles(List<Vector3Int> tiles)
    {
        StopWandering();

        assignedTiles = new List<Vector3Int>(tiles);
        currentTileIndex = 0;

        if (assignedTiles.Count == 0) return;

        state = RobotState.WalkingToTile;
        StartCoroutine(ExecuteTasks());
    }

    IEnumerator ExecuteTasks()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        while (currentTileIndex < assignedTiles.Count)
        {
            var tileCell = assignedTiles[currentTileIndex];
            Vector3 worldPos = groundTilemap != null
                ? groundTilemap.GetCellCenterWorld(tileCell)
                : (Vector3)tileCell;

            state = RobotState.WalkingToTile;
            yield return StartCoroutine(WalkToManhattan(worldPos));

            state = RobotState.Working;
            yield return new WaitForSeconds(definition.secondsPerTile);

            PerformTaskOnTile(tileCell, worldPos);
            currentTileIndex++;
        }

        state = RobotState.Idle;
        assignedTiles.Clear();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        StartWandering();
    }

    IEnumerator WalkTo(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        string walkAnim = GetWalkAnimation(dir);

        float timeout = 5f;
        float elapsed = 0f;
        Vector3 lastPos = transform.position;
        float stuckTimer = 0f;

        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName(walkAnim))
                anim.Play(walkAnim);

            transform.position = Vector3.MoveTowards(
                transform.position, target,
                definition.moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, lastPos) < 0.001f)
                stuckTimer += Time.deltaTime;
            else
                stuckTimer = 0f;

            lastPos = transform.position;
            elapsed += Time.deltaTime;

            // give up if stuck for 0.5s or total timeout exceeded
            if (stuckTimer > 0.5f || elapsed > timeout)
            {
                Debug.Log($"{definition.robotName} stuck, skipping to next target");
                break;
            }

            yield return null;
        }

        transform.position = Vector3.Distance(transform.position, target) < 0.5f
            ? target
            : transform.position; // stays where he is if he couldn't reach it
    }

    IEnumerator WalkToManhattan(Vector3 target)
    {
        // First move horizontally, then vertically
        Vector3 midPoint = new Vector3(target.x, transform.position.y, 0);

        if (Vector2.Distance(transform.position, midPoint) > 0.1f)
            yield return StartCoroutine(WalkTo(midPoint));

        if (Vector2.Distance(transform.position, target) > 0.1f)
            yield return StartCoroutine(WalkTo(target));
    }

    void PerformTaskOnTile(Vector3Int cell, Vector3 worldPos)
    {
        switch (definition.type)
        {
            case RobotType.Digger:
                DigTile(cell);
                break;

            case RobotType.Planter:
                PlantOnTile(cell, worldPos);
                break;
        }
    }

    void DigTile(Vector3Int cell)
    {
        if (dugTilemap == null) return;
        dugTilemap.SetTile(cell, RobotManager.Instance?.dugSoilTile);
        //TODO audio
        //AudioManager.Instance?.PlaySFX(AudioManager.Instance.digSFX);
    }

    void PlantOnTile(Vector3Int cell, Vector3 worldPos)
    {
        if (plantPrefab == null) return;

        // can plant on dug tiles
        if (dugTilemap != null && dugTilemap.GetTile(cell) == null)
        {
            Debug.Log("Skipping tile — not dug yet");
            return;
        }

        // nothing already planted
        var hits = Physics2D.OverlapCircleAll(worldPos, 0.3f);
        foreach (var hit in hits)
            if (hit.GetComponent<LuminescentPlant>() != null) return;

        var plant = Instantiate(plantPrefab, worldPos, Quaternion.identity);

        var lumPlant = plant.GetComponent<LuminescentPlant>();
        if (lumPlant != null)
            lumPlant.ForceStage(0);
    }

    string GetWalkAnimation(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            sr.flipX = dir.x < 0;
            return "walking-side";
        }
        else
        {
            sr.flipX = false;
            return dir.y < 0 ? "walking-front" : "walking-back";
        }
    }

    public void StartWandering()
    {
        if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
        wanderCoroutine = StartCoroutine(WanderLoop());
    }

    public void StopWandering()
    {
        if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
        wanderCoroutine = null;
        rb.linearVelocity = Vector2.zero;
    }
    IEnumerator WanderLoop()
    {
        Vector3 origin = transform.position;

        while (true)
        {
            float wait = Random.Range(wanderWaitMin, wanderWaitMax);
            yield return new WaitForSeconds(wait);

            if (state != RobotState.Idle) yield break;

            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 target = origin + new Vector3(offset.x, offset.y, 0);

            state = RobotState.Wandering;
            yield return StartCoroutine(WalkToManhattan(target));

            state = RobotState.Idle;
            if (anim != null) anim.enabled = false;
        }
    }
}
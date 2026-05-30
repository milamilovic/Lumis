using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public enum RobotState { Idle, Working, WalkingToTile }

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
    }

    public void AssignTiles(List<Vector3Int> tiles)
    {
        assignedTiles = new List<Vector3Int>(tiles);
        currentTileIndex = 0;

        if (assignedTiles.Count == 0) return;

        state = RobotState.WalkingToTile;
        StartCoroutine(ExecuteTasks());
    }

    IEnumerator ExecuteTasks()
    {
        while (currentTileIndex < assignedTiles.Count)
        {
            var tileCell = assignedTiles[currentTileIndex];
            Vector3 worldPos = groundTilemap != null
                ? groundTilemap.GetCellCenterWorld(tileCell)
                : (Vector3)tileCell;

            state = RobotState.WalkingToTile;
            yield return StartCoroutine(WalkTo(worldPos));

            state = RobotState.Working;
            yield return new WaitForSeconds(definition.secondsPerTile);

            PerformTaskOnTile(tileCell, worldPos);
            currentTileIndex++;
        }

        state = RobotState.Idle;
        assignedTiles.Clear();
    }

    IEnumerator WalkTo(Vector3 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            string walkAnim = GetWalkAnimation(target);
            if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName(walkAnim) == false)
                anim.Play(walkAnim);

            transform.position = Vector3.MoveTowards(
                transform.position, target,
                definition.moveSpeed * Time.deltaTime);

            yield return null;
        }
        transform.position = target;
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
        if (dugTilemap != null && dugTilemap.GetTile(cell) == null) return;
        Instantiate(plantPrefab, worldPos, Quaternion.identity);
    }

    void OnMouseDown()
    {
        TaskAssignmentManager.Instance?.SelectRobot(this);
    }

    string GetWalkAnimation(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;

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
}
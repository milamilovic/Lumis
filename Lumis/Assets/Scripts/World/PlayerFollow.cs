using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    [Header("Pixel Snapping")]
    public float pixelsPerUnit = 32f;

    void Start()
    {
        FindTarget();
    }

    void FindTarget()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        Vector3 desired = target.position + offset;
        desired.x = Mathf.Round(desired.x * pixelsPerUnit) / pixelsPerUnit;
        desired.y = Mathf.Round(desired.y * pixelsPerUnit) / pixelsPerUnit;
        desired.z = offset.z;
        transform.position = desired;
    }
}
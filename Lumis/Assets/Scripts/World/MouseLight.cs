using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class MouseLight : MonoBehaviour
{
    public Light2D light2D;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnDestroy()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector3 mouseScreen = new Vector3(
            mouse.position.x.ReadValue(),
            mouse.position.y.ReadValue(),
            0f
        );

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;
        transform.position = mouseWorld;
    }
}
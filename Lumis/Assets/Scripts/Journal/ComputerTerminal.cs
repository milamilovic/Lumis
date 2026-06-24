using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ComputerTerminal : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    void Update()
    {
        if (!playerInRange) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (JournalManager.Instance != null && JournalManager.Instance.IsTerminalOpen)
                return;

            JournalManager.Instance?.OpenTerminal();
        }
    }
}
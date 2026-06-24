using UnityEngine;

[CreateAssetMenu(fileName = "JournalEntry", menuName = "Game/Journal Entry")]
public class JournalEntry : ScriptableObject
{
    public string entryTitle;
    public string diskItemId;   //inventory item that unlocks this one
    [TextArea(4, 12)]
    public string entryText;
    public int unlockOrder; // 0 for always available, 1+ unlock in sequence
    public AudioClip ambientSound;

    [Header("Password lock")]
    public bool isLocked;
    public string password;          // shown in previous entry
    public string passwordHint;
}
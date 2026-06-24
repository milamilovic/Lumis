using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance;

    [Header("All entries in game")]
    public List<JournalEntry> allEntries;

    [Header("Terminal Panel")]
    public GameObject terminalPanel;
    public Transform diskSlotListParent;
    public GameObject diskSlotButtonPrefab;

    [Header("Entry Content")]
    public TextMeshProUGUI entryTitleLabel;
    public TextMeshProUGUI entryBodyLabel;

    [Header("Password UI")]
    public GameObject passwordInputPanel;
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI passwordHintLabel;
    public TextMeshProUGUI passwordErrorLabel;
    public Button submitPasswordButton;

    [Header("Audio")]
    public AudioClip pageFlipSFX;
    public AudioClip wrongPasswordSFX;
    public AudioClip correctPasswordSFX;

    private HashSet<string> discoveredEntries = new();

    private JournalEntry pendingEntry;
    private Inventory inventory;

    public bool IsTerminalOpen => terminalPanel != null && terminalPanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        terminalPanel.SetActive(false);
        passwordInputPanel.SetActive(false);

        if (submitPasswordButton != null)
            submitPasswordButton.onClick.AddListener(OnSubmitPassword);

        inventory = FindFirstObjectByType<Inventory>();

        ClearEntryView();
    }

    public void OpenTerminal()
    {
        terminalPanel.SetActive(true);
        passwordInputPanel.SetActive(false);
        pendingEntry = null;

        Time.timeScale = 0f;

        ClearEntryView();
        StartCoroutine(RefreshDiskListDelayed());
    }

    IEnumerator RefreshDiskListDelayed()
    {
        yield return null;
        RefreshDiskList();
    }

    public void CloseTerminal()
    {
        terminalPanel.SetActive(false);
        passwordInputPanel.SetActive(false);
        pendingEntry = null;

        Time.timeScale = 1f;
    }

    private void ClearEntryView()
    {
        if (entryTitleLabel != null)
            entryTitleLabel.text = "";

        if (entryBodyLabel != null)
            entryBodyLabel.text = "";
    }

    private void RefreshDiskList()
    {
        Debug.Log($"RefreshDiskList called. allEntries.Count = {allEntries.Count}");
        foreach (Transform child in diskSlotListParent)
            Destroy(child.gameObject);

        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();

        Debug.Log($"inventory = {inventory}");

        if (inventory == null) return;

        foreach (var entry in allEntries)
        {
            int owned = CountInInventory(inventory, entry.diskItemId);
            if (owned <= 0) continue;

            GameObject btn = Instantiate(diskSlotButtonPrefab, diskSlotListParent);

            TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = entry.entryTitle;

            Button button = btn.GetComponent<Button>();
            Debug.Log($"Entry: {entry.entryTitle}, Button found: {button}");
            JournalEntry capturedEntry = entry;
            if (button != null)
                button.onClick.AddListener(() => {
                    Debug.Log($"Clicked: {capturedEntry.entryTitle}");
                    TryOpenEntry(capturedEntry);
                });
        }
    }

    private void TryOpenEntry(JournalEntry entry)
    {
        AudioManager.Instance?.PlaySFX(pageFlipSFX);

        bool discovered = discoveredEntries.Contains(GetEntryKey(entry));

        if (entry.isLocked && !discovered)
        {
            pendingEntry = entry;

            passwordInputPanel.SetActive(true);

            if (passwordHintLabel != null)
                passwordHintLabel.text = entry.passwordHint;

            if (passwordErrorLabel != null)
                passwordErrorLabel.text = "";

            if (passwordInputField != null)
            {
                passwordInputField.text = "";
                passwordInputField.ActivateInputField();
            }

            return;
        }

        passwordInputPanel.SetActive(false);
        ShowEntryContent(entry);
    }

    private void OnSubmitPassword()
    {
        if (pendingEntry == null)
            return;

        string enteredPassword = passwordInputField != null
            ? passwordInputField.text.Trim()
            : "";

        string correctPassword = pendingEntry.password != null
            ? pendingEntry.password.Trim()
            : "";

        if (enteredPassword == correctPassword)
        {
            AudioManager.Instance?.PlaySFX(correctPasswordSFX);

            discoveredEntries.Add(GetEntryKey(pendingEntry));
            passwordInputPanel.SetActive(false);

            ShowEntryContent(pendingEntry);
            pendingEntry = null;
        }
        else
        {
            AudioManager.Instance?.PlaySFX(wrongPasswordSFX);

            if (passwordErrorLabel != null)
                passwordErrorLabel.text = "Incorrect password. Try again.";
        }
    }

    private void ShowEntryContent(JournalEntry entry)
    {
        discoveredEntries.Add(GetEntryKey(entry));

        if (entryTitleLabel != null)
            entryTitleLabel.text = entry.entryTitle;

        if (entryBodyLabel != null)
            entryBodyLabel.text = entry.entryText;

        RefreshDiskList();
    }

    private int CountInInventory(Inventory inv, string itemId)
    {
        if (inv == null || string.IsNullOrEmpty(itemId))
            return 0;

        foreach (var item in inv.items)
        {
            if (item != null && item.id == itemId)
                return item.quantity;
        }

        return 0;
    }

    private string GetEntryKey(JournalEntry entry)
    {
        return entry.name;
    }
}
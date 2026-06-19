using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnId;

    void Start()
    {
        string target = PlayerPrefs.GetString("SpawnPoint", "");
        if (target == spawnId)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.transform.position = transform.position;
                player.transform.localScale = Vector3.one;
            }
            PlayerPrefs.DeleteKey("SpawnPoint");
        }
    }
}
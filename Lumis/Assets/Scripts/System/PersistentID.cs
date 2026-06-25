using UnityEngine;

public class PersistentID : MonoBehaviour
{
    [SerializeField] private string _id;

    public string ID
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
                Debug.LogWarning($"{gameObject.name}: PersistentID was EMPTY, generated NEW one: {_id}");
            }
            return _id;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Generate New ID")]
    void GenerateNewID()
    {
        _id = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        Debug.Log($"{gameObject.name}: Generated new ID = {_id}");
    }

    void OnValidate()
    {
        if (string.IsNullOrEmpty(_id))
        {
            _id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);

            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager
                    .MarkSceneDirty(gameObject.scene);
            }
        }
    }
#endif
}
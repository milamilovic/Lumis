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
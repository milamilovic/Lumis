using UnityEngine;
using System.Collections;

public class InteriorBootstrap : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        yield return new WaitForSeconds(0.1f);
        if (SceneFader.Instance != null)
            yield return StartCoroutine(SceneFader.Instance.FadeOut());
    }
}
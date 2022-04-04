using System.Collections;
using Enums;
using UnityEngine;
using Utils;

public class TriggerTutorial : MonoBehaviour
{
    private SceneUtils _sceneUtils;

    // Start is called before the first frame update
    void Start()
    {
        _sceneUtils = GetComponent<SceneUtils>();
        AudioManager.Instance.StartSound(Sfx.TutorialText);
        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(40);
        _sceneUtils.LoadNextScene();
    }
}
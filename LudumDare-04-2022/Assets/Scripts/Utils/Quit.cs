using UnityEngine;

namespace Utils
{
    public class Quit : MonoBehaviour
    {
        public void QuitGame()
        {
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
    Application.Quit();
#elif (UNITY_WEBGL)
    Application.OpenURL("about:blank");
#endif
        }
    }
}
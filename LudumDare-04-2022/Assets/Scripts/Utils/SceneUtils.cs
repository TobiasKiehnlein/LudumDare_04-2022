using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneUtils : MonoBehaviour
    {
        [SerializeField] private Animator crossFade;

        private static readonly int Start = Animator.StringToHash("Start");

        public void LoadNextScene()
        {
            var scene = SceneManager.GetActiveScene();
            var nextLevelBuildIndex = (scene.buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            StartCoroutine(LoadScene(nextLevelBuildIndex));
        }

        private IEnumerator LoadScene(int sceneIndex)
        {
            crossFade.SetTrigger(Start);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadMenuScene()
        {
            Time.timeScale = 1;
            StartCoroutine(LoadScene(0));
        }
    }
}
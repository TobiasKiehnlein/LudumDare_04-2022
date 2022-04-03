using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneUtils : MonoBehaviour
    {
        [SerializeField]
        private Animator crossFade;

        public void LoadNextScene()
        {
            var scene = SceneManager.GetActiveScene();
            var nextLevelBuildIndex = (scene.buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            LoadScene(nextLevelBuildIndex);
        }

        private async void LoadScene(int sceneIndex)
        {
            crossFade.SetTrigger("Start");
            await Task.Delay(1000);
            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadMenuScene()
        {
            LoadScene(0);
        }
    
    
    }
}
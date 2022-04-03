using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class ActivateOnStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Image>().enabled = true;
        }
    }
}
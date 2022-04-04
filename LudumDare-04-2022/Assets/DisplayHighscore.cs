using TMPro;
using UnityEngine;

public class DisplayHighscore : MonoBehaviour
{
    private TMP_Text _tmp;

    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _tmp.text = ((int) GameManager.Instance.Score).ToString();
    }
}
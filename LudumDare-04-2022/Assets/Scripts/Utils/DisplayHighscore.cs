using System;
using TMPro;
using UnityEngine;

public class DisplayHighscore : MonoBehaviour
{
    private TMP_Text _tmp;
    [SerializeField] private bool displayCurrentScore = true;
    private float _highScore;

    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponent<TMP_Text>();
        _highScore = PlayerPrefs.GetFloat("HighScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        _tmp.text = ((int) (displayCurrentScore ? GameManager.Instance.Score : Math.Max(_highScore, GameManager.Instance.Score))).ToString();
    }
}
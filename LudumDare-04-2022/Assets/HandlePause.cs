using System;
using UnityEngine;

public class HandlePause : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        var isPaused = Math.Abs(Time.timeScale - 1) < .1; 
        Time.timeScale = isPaused ? 0 : 1;

        _canvas.enabled = isPaused;
    }
}
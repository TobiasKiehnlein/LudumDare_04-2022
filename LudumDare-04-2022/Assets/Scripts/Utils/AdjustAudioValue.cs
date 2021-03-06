using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class AdjustAudioValue : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private bool isMusic;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
    }

    private void Update()
    {
        _slider.value = isMusic ? settings.MusicVolume : settings.SfxVolume;
    }

    public void SetLoudness(float value)
    {
        if (isMusic)
            AudioManager.Instance.SetMusicVolume(value);
        else
            AudioManager.Instance.SetSfxVolume(value);
    }
}
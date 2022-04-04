using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class HandleMute : MonoBehaviour
{
    [SerializeField] private Sprite active;
    [SerializeField] private Sprite muted;
    [SerializeField] private bool isMusic;

    private Image _image;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
    }

    private void SetSprite(bool isMuted)
    {
        _image.sprite = isMuted ? muted : active;
    }

    // Update is called once per frame
    void Update()
    {
        SetSprite(isMusic ? AudioManager.Instance.MusicMute : AudioManager.Instance.SfxMute);
    }

    public void HandleClick()
    {
        if (isMusic)
        {
            AudioManager.Instance.SetMusicMute(!AudioManager.Instance.MusicMute);
        }
        else
        {
            AudioManager.Instance.SetSfxMute(!AudioManager.Instance.SfxMute);
        }
    }
}
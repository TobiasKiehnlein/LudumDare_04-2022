using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "CustomGameSettings/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private float musicVolume = .6f;
        [SerializeField] private float sfxVolume = .8f;

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = value;
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
            }
        }

        public float SfxVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = value;
                PlayerPrefs.SetFloat("SfxVolume", value);
                PlayerPrefs.Save();
            }
        }
        private void OnEnable()
        {
            musicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1;
            sfxVolume = PlayerPrefs.HasKey("SfxVolume") ? PlayerPrefs.GetFloat("SfxVolume") : 1;
        }
    }
}
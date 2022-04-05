using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "CustomGameSettings/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private float musicVolume = .6f;
        [SerializeField] private float sfxVolume = 1f;

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
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", .7f);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1);
        }
    }
}
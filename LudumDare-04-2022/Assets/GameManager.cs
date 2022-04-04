using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using ScriptableObjects;
using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    internal struct HumanInfo
    {
        public float SpawnTime;
        public float? DeathTime;
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameObject> deathPrefabs;
    [SerializeField] private List<GameObject> peoplePrefabs;
    private List<Transform> _spawnLocationsDeaths;
    private List<Transform> _spawnLocationsPeople;

    [SerializeField] private float deathSpawnRateInSeconds = 15;
    [SerializeField] private int peopleCount = 25;

    [SerializeField] private float scoreScale = 1;

    private float _lastSpawn;
    private bool _firstDeath = true;

    public float Score { get; private set; }

    private readonly Dictionary<int, HumanInfo> _data = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _spawnLocationsPeople = FindObjectsOfType<PlayerSpawnPoint>().Select(point => point.gameObject.transform).ToList();
        _spawnLocationsDeaths = FindObjectsOfType<DeathSpawnPoint>().Select(point => point.gameObject.transform).ToList();
        _lastSpawn = -0.5f * deathSpawnRateInSeconds;
        AudioManager.Instance.StartSound(Music.Major, 3f);
        for (var i = 0; i < peopleCount; i++)
        {
            var person = Instantiate(peoplePrefabs.GetRandomElement());
            person.transform.position = (Vector2) _spawnLocationsPeople.GetRandomElement().transform.position;
        }
    }

    private void Update()
    {
        if (_lastSpawn + deathSpawnRateInSeconds < Time.time)
        {
            _lastSpawn = Time.time;
            var death = Instantiate(deathPrefabs.GetRandomElement());
            death.transform.position = (Vector2) _spawnLocationsDeaths.GetRandomElement().transform.position;
            Debug.Log("Spawning");
        }

        Score += _data.Values.Where(d => d.DeathTime == null).Select(_ => Time.deltaTime).Sum() * scoreScale;
    }

    public void RegisterDeath(int instanceId)
    {
        if (_firstDeath)
        {
            AudioManager.Instance.StartSound(Music.Minor1);
            //TODO trigger bullet time and camera zoom
            _firstDeath = false;
        }
        else
        {
            var percentageAlive = (float) _data.Values.Count(d => d.DeathTime == null) / _data.Count * 100;
            switch (percentageAlive)
            {
                case > 90:
                    AudioManager.Instance.StartSound(Music.Minor1);
                    break;
                case > 70:
                    AudioManager.Instance.StartSound(Music.Minor2, 5);
                    break;
                case > 50:
                    AudioManager.Instance.StartSound(Music.Minor3, 10);
                    break;
                case > 15:
                    AudioManager.Instance.StartSound(Music.Minor4, 10);
                    break;
                default:
                    AudioManager.Instance.StartSound(Music.Minor5);
                    break;
            }
        }

        _data[instanceId] = new HumanInfo {SpawnTime = _data[instanceId].SpawnTime, DeathTime = Time.time};
    }

    public void RegisterHuman(int instanceId)
    {
        _data.Add(instanceId, new HumanInfo {SpawnTime = Time.time});
    }
}
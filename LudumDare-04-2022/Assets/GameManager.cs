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
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameObject> deathPrefabs;
    [SerializeField] private List<GameObject> peoplePrefabs;
    private List<Transform> _spawnLocationsDeaths;
    private List<Transform> _spawnLocationsPeople;

    [SerializeField] private float deathSpawnRateInSeconds = 15;
    [SerializeField] private int peopleCount = 25;
    private float _lastSpawn;

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
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _randomEnemyPrefab;
    [SerializeField] private int _enemiesToSpawn = 5;
    [SerializeField] private int _enemiesSpawned = 0;
    [SerializeField] private GameObject _bossPrefab;
    public int enemiesKilled;
    private UIManager _uiManager;
    private bool _isWaveAnnouncing = true;
    public int _nextWave = 1;
    private bool _isBossAnnouncing = true;
    private int _nextBoss = 1;
    private Player _player;
    public bool _stopSpawning = false;
    public int[] enemyTable =
    {
        700,    // Double Laser Enemy
        300     // Laserbeam Enemy
    };
    public int enemyTotal;
    public int randomEnemyNumber;
    private bool _enemySpawnRestart;

    public int[] powerupTable =
    {
        400,    // Ammo 
        225,    // Speed Boost
        175,    // Health
        140,    // Shields
        105,    // Triple Shot
        75,     // Speed Reduction
        55,     // Bombs
        45      // Homing Missles

    };
    public int puTotal;
    public int randomPuNumber;
    private bool _powerupSpawnRestart;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        foreach (var item in powerupTable)
        {
            puTotal += item;
        }

        foreach (var item in enemyTable)
        {
            enemyTotal += item;
        }

    }

    private void Update()
    {
        Wave();
        RestartSpawnPowerupRoutine();
        RestartSpawnEnemyRoutine();
    }

    private void Wave()
    {
        if (_enemiesSpawned == _enemiesToSpawn)
        {
            StopCoroutine(SpawnEnemyRoutine());
        }
        if (enemiesKilled >= _enemiesToSpawn)
        {
            StopAllCoroutines();
            enemiesKilled = 0;
            _enemiesSpawned = 0;
            _enemiesToSpawn = (int)Mathf.Round(_enemiesToSpawn + _enemiesToSpawn / 1.5f);
            _nextWave++;
            _isWaveAnnouncing = true;
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (_player != null)
        {
            if (_nextWave < 4)
            {
                if (_isWaveAnnouncing != false)
                {
                    StartCoroutine(AnnounceWave());
                }
                else
                {
                    _stopSpawning = false;
                    StartCoroutine(SpawnEnemyRoutine());
                    StartCoroutine(SpawnPowerupRoutine());
                }
            }
            if (_nextWave >= 4)
            {
                _stopSpawning = true;
                _isBossAnnouncing = true;
                StartCoroutine(AnnounceBoss());
            }
        }
    }

    IEnumerator AnnounceBoss()
    {
        _uiManager.UpdateBoss(_nextBoss);
        yield return new WaitForSeconds(1.0f);
        _uiManager._announceBoss.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _uiManager._announceBoss.gameObject.SetActive(false);
        _isBossAnnouncing = false;
        StartCoroutine(SpawnBossRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator AnnounceWave()
    {
        _uiManager.UpdateWave(_nextWave);
        yield return new WaitForSeconds(1.0f);
        _uiManager._announceWave.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _uiManager._announceWave.gameObject.SetActive(false);
        _isWaveAnnouncing = false;
        StartSpawning();
    }

    IEnumerator SpawnBossRoutine()
    {
        yield return new WaitForSeconds(2.4f);
        Vector3 posToSpawn = new Vector3(0, 7.12f, 0);
        GameObject newBoss = Instantiate(_bossPrefab, posToSpawn, Quaternion.identity);
        newBoss.transform.parent = _enemyContainer.transform;

    }
    
    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false && _enemiesSpawned < _enemiesToSpawn)
        {
            yield return new WaitForSeconds(2.4f);
            randomEnemyNumber = Random.Range(0, enemyTotal);

            for(int i =0; i < enemyTable.Length; i++)
            {
                if (randomEnemyNumber <= enemyTable[i])
                {
                    _enemiesSpawned++;
                    Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.12f, 0);
                    GameObject newEnemy = Instantiate(_randomEnemyPrefab[i], posToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    _enemySpawnRestart = true;
                    yield break;
                }
                else
                {
                    randomEnemyNumber -= enemyTable[i];
                }
            }
            yield return new WaitForSeconds(4.0f);
        }
    }

    private void RestartSpawnEnemyRoutine()
    {
        if (_enemySpawnRestart == true)
        {
            _enemySpawnRestart = false;
            StartCoroutine(SpawnEnemyRoutine());
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false || (_nextWave >= 4 && _bossPrefab != null))
        {
            yield return new WaitForSeconds(7.0f);
            randomPuNumber = Random.Range(0, puTotal);

            for (int i = 0; i < powerupTable.Length; i++)
            {
                if (randomPuNumber <= powerupTable[i])
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.12f, 0);
                    Instantiate(_powerups[i], posToSpawn, Quaternion.identity);
                    _powerupSpawnRestart = true;
                    yield break;
                }
                else
                {
                    randomPuNumber -= powerupTable[i];
                }
            }
            yield return new WaitForSeconds(Random.Range(3, 7));
        }
    }

    public void RestartSpawnPowerupRoutine()
    {
        if (_powerupSpawnRestart == true)
        {
            _powerupSpawnRestart = false;
            StartCoroutine(SpawnPowerupRoutine());
        }
    }

    public void OnPlayerDeath()
    {
        StopAllCoroutines();        
    }
}

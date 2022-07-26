using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //[SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _rarePowerups;
    [SerializeField] private GameObject[] _randomEnemyPrefab;
    [SerializeField] private int _enemiesToSpawn = 16;
    [SerializeField] private int _enemiesSpawned = 0;
    public int enemiesKilled;
    private UIManager _uiManager;
    private bool _isWaveAnnouncing = true;
    private int _nextWave = 1;
    private Player _player;
    public bool _stopSpawning = false;
    public int[] powerupTable =
    {
        260,    // Ammo 
        190,    // Speed Boost
        160,    // Shields
        145,    // Health
        105,    // Triple Shot
        95,     // Speed Reduction
        45      // Bombs

    };
    public int puTotal;
    public int randomPuNumber;
    private bool _powerupSpawn;

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
            Debug.LogError("The UI Manager is NULL.");
        }

        foreach (var item in powerupTable)
        {
            puTotal += item;
        }

    }

    private void Update()
    {
        Wave();
        RestartSpawnPowerupRoutine();
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
        if (_isWaveAnnouncing != false && _player != null)
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

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2.4f);
        while (_stopSpawning == false && _enemiesSpawned < _enemiesToSpawn)
        {
            _enemiesSpawned++;
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.12f, 0);

            //Using this one until I figure out issues with other enemy
            int randomEnemy = Random.Range(0, 1);
            //int randomEnemy = Random.Range(0, 2);
            GameObject newEnemy = Instantiate(_randomEnemyPrefab[randomEnemy], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {

        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(10.0f);

            randomPuNumber = Random.Range(0, puTotal);

            for (int i = 0; i < powerupTable.Length; i++)
            {
                if (randomPuNumber <= powerupTable[i])
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.12f, 0);
                    Instantiate(_powerups[i], posToSpawn, Quaternion.identity);
                    _powerupSpawn = true;
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

    private void RestartSpawnPowerupRoutine()
    {
        if (_powerupSpawn == true)
        {
            _powerupSpawn = false;
            StartCoroutine(SpawnPowerupRoutine());
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}

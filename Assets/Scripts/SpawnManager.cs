using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _rarePowerups;

    private int _nextWave = 1;
    [SerializeField] private int _enemiesToSpawn = 20;
    [SerializeField] private int _enemiesSpawned = 0;
    public int enemiesKilled;
    private UIManager _uiManager;
    private bool _isWaveAnnouncing = true;
    private Player _player;

    public bool _stopSpawning = false;

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
    }

    private void Update()
    {
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
            StartCoroutine(SpawnRarePowerupRoutine());
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
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(10.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.12f, 0);
            int randomPowerUp = Random.Range(0, 5);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 7));
        }
    }

    IEnumerator SpawnRarePowerupRoutine()
    {
        yield return new WaitForSeconds(20.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 7.12f, 0);
            int randomRarePowerUp = Random.Range(0, 1);
            Instantiate(_rarePowerups[randomRarePowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(25, 35));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _fireRate = 0.25f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _score;
    [SerializeField] private int _shieldStrength = 0;
    [SerializeField] private int _ammoCount = 25;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioClip _ammoEmptyClip;
    [SerializeField] private AudioClip _bombBeepClip;
    [SerializeField] public CameraShake _cameraShake;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private bool _isTripleShotActive = false;
    private bool _isBombsActive = false;
    private float _speedMultiplier = 2.25f;
    private float _canFire = -1f;
    private float _thrusterLevel = 1f;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }
        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource on the PLayer is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && (_thrusterLevel < 50f))
            {
                transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
                ThrusterCounter();
            }
            else
            {
                transform.Translate(direction * _speed * Time.deltaTime);
                if (!Input.GetKey(KeyCode.LeftShift))
                    ThrusterRefill();
            }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.5f, 9.5f), Mathf.Clamp(transform.position.y, -3.15f, 2f), 0);
    }

    void ThrusterCounter()
    {
        if (_thrusterLevel > 0f && _thrusterLevel < 50f)
        {
            _uiManager.UpdateThrusterLevel(_thrusterLevel);
            _thrusterLevel += .025f;
        }

        if (_thrusterLevel == 50f)
        {
            _thrusterLevel = 0f;
        }
    }

    void ThrusterRefill()
    {
        if (_thrusterLevel > 1)
        {
            _uiManager.UpdateThrusterLevel(_thrusterLevel);
            _thrusterLevel -= .006f;
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_ammoCount < 1)
        {
            _audioSource.clip = _ammoEmptyClip;
            _audioSource.Play();
            return;
        }
        else if (_ammoCount > 0 && _isBombsActive == true)
        {
            _audioSource.clip = _bombBeepClip;
            _audioSource.Play();
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
            _ammoCount--;
        }
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isBombsActive == true)
        {
            Instantiate(_bombPrefab, transform.position + new Vector3(0, 0.87f, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.87f, 0), Quaternion.identity);
        }
        _audioSource.Play();
    }
    public void Damage()
    {
        StartCoroutine(_cameraShake.Shake(.15f, .4f));

        if (_shieldStrength > 1)
        {
            _shieldStrength--;
            _uiManager.UpdateShieldStrength(_shieldStrength);
            return;
        }
        else if (_shieldStrength == 1)
        {
            _shieldStrength--;
            _shieldVisualizer.SetActive(false);
            _uiManager.UpdateShieldStrength(_shieldStrength);
            return;
        }
        _lives--;
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
    public void HealthCollected()
    {
        if (_lives == 1)
        {
            _lives++;
            _rightEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _lives++;
            _leftEngine.SetActive(false);
        }
        _uiManager.UpdateLives(_lives);
    }
    public void TripleShotActive()
    {
        _isBombsActive = false;
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        _isTripleShotActive = false;
    }
    public void SpeedBoostCollected()
    {
        _thrusterLevel = 1;
        _uiManager.UpdateThrusterLevel(_thrusterLevel);
    }
    public void ShieldsActive()
    {
        _shieldStrength = 3;
        _uiManager.UpdateShieldStrength(_shieldStrength);
        _shieldVisualizer.SetActive(true);
    }
    public void AmmoCollected()
    {
        _ammoCount = 25;
    }
    public void BombsCollected()
    {
        _isTripleShotActive = false;
        _isBombsActive = true;
        StartCoroutine(BombPowerDown());
    }
    IEnumerator BombPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isBombsActive = false;
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
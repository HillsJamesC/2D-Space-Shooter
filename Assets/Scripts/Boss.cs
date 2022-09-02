using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private GameObject _bombExplosionPrefab;
    [SerializeField] private GameObject _bossLaserBeam;
    [SerializeField] private GameObject _bossLaserPrefab;
    [SerializeField] private int _bossHealth = 1000;
    private Collider2D _bossCollider;
    private AudioSource _audioSource;
    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    private Animator _anim;
    private Vector3 _newPosition;
    private Player _player;
    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;
    private bool _canMove = true;


    // Start is called before the first frame update
    void Start()
    {

        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _bossCollider = GetComponent<Collider2D>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _newPosition = new Vector3(0, 1.75f, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _spawnManager.RestartSpawnPowerupRoutine();
        
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        if (_bossCollider == null)
        {
            Debug.LogError("The Collider2D is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The Animator is Null.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            Destroy(this.gameObject);
        }

        if (_canMove == true)
        {
            CalculateMovement();
            CanFire();
        }
    }

    private void CalculateMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, _newPosition, _speed * Time.deltaTime);

        if (transform.position == _newPosition)
        {
            _canMove = false;
            StartCoroutine(PauseMovement());
        }
    }

    

    IEnumerator PauseMovement()
    {
        yield return new WaitForSeconds(0.25f);
        _newPosition = new Vector3(Random.Range(-4.83f, 4.83f), Random.Range(1.2f, 3.5f), 0);
        _speed = 3.5f;
        _canMove = true;
    }

    private void CanFire()
    {
        if (Time.time > _canFire)
        {
            GameObject enemyLaser = Instantiate(_bossLaserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }

            StartCoroutine(ActivateLaserBeam());
            _fireRate = Random.Range(3f, 6f);
            _canFire = Time.time + _fireRate;
        }
    }

    IEnumerator ActivateLaserBeam()
    {
        yield return new WaitForSeconds(1.0f);
        _bossLaserBeam.SetActive(true);
        yield return new WaitForSeconds(2.25f);
        _bossLaserBeam.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_player != null)
        {
            if (other.CompareTag("Player"))
            {
                _player.Damage();
                _bossHealth -= 1;
            }

            if (other.CompareTag("Laser"))
            {
                Destroy(other.gameObject);
                _player.AddScore(10);
                _bossHealth -= 20;
            }

            if (other.CompareTag("Bomb"))
            {
                _player.AddScore(20);
                Destroy(other.gameObject);
                _bossHealth -= 45;
            }

            if (other.CompareTag("HomingMissle"))
            {
                Destroy(other.gameObject);
                _player.AddScore(75);
                _bossHealth -= 55;
            }

            if(_bossHealth <= 0)
            {
                BossDestroyed();
            }
        }

        void BossDestroyed()
        {
            _canFire = 1;
            _bossCollider.enabled = false;
            _anim.SetTrigger("OnBossDeath");
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
            _uiManager.GameOverSequence();
        }
    }
}

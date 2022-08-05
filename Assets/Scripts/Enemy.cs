using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3.2f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserBeam;
    [SerializeField] private GameObject _bombExplosionPrefab;
    [SerializeField] private GameObject _enemyShieldVisualizer;
    [SerializeField] private float _enemyRamDistance = 3.2f;
    [SerializeField] private float _enemyRamSpeed = 2f;
    [SerializeField] private GameObject _enemyBackShot;
    [SerializeField] private float _enemyBackShotDistance = 4.2f;
    private SpriteRenderer _spriteRenderer;
    private Player _player;
    private Animator _anim;
    private Collider2D _enemyCollider;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private int _randomMovement; //0 = Down, 1 = Wave
    private bool _isEnemyShieldActive;
    private int _randomEnemyShield;

    void Start()
    {
        EnemyShields();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _enemyCollider = GetComponent<Collider2D>();
        _randomMovement = Random.Range(0, 2);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source is NULL.");
        }

        if (_enemyCollider == null)
        {
            Debug.LogError("The Collider2D is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is Null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            Destroy(this.gameObject);
        }

        CalculateMovement();
        CanFire();
    }

    private void CanFire()
    {
        if (Time.time > _canFire)
        {

            _fireRate = Random.Range(2f, 5f);
            _canFire = Time.time + _fireRate;

            if (transform.CompareTag("Enemy"))
            {
                GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignEnemyLaser();
                }
                if (Vector3.Distance(transform.position, _player.transform.position) <= _enemyBackShotDistance)
                {
                    if (transform.position.y < _player.transform.position.y)
                    {
                        GameObject enemyBackShot = Instantiate(_enemyBackShot, transform.position, Quaternion.identity);
                        Laser backShot = enemyBackShot.GetComponent<Laser>();
                        backShot.AssignEnemyBackShot();
                    }
                }
            }

            if (transform.CompareTag("Enemy2"))
            {
                _laserBeam.SetActive(true);
                StartCoroutine(DeactivateLaserBeam());
            }
        }
    }

    private void EnemyShields()
    {
        _randomEnemyShield = Random.Range(0, 5);

        if (_randomEnemyShield > 4)
        {
            _isEnemyShieldActive = false;
        }
        else
        {
            _isEnemyShieldActive = true;
            _enemyShieldVisualizer.SetActive(true);
        }

    }

    IEnumerator DeactivateLaserBeam()
    {
        yield return new WaitForSeconds(1f);
        _laserBeam.SetActive(false);
    }

    void CalculateMovement()
    {
        // Using Switch statement to later add more movements
        switch (_randomMovement)
        {
            case 1:
                transform.Translate(_speed * Time.deltaTime * new Vector3(Mathf.Cos(Time.time * 4) * 2, -1, 0));
                break;
            //case 2:
            //case 3:
            default:
                transform.Translate(_speed * Time.deltaTime * Vector3.down);
                break;
        }

        if (_player != null)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= _enemyRamDistance)
            {
                if (transform.position.y > _player.transform.position.y)
                {
                    StartCoroutine(EnemyAggressionColor());
                    transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _enemyRamSpeed * Time.deltaTime);
                }
            }
            else
            {
                StopCoroutine(EnemyAggressionColor());
                _spriteRenderer.color = Color.white;
            }
        }

        if (_enemyCollider.enabled == false && transform.position.y < -4.92f)
        {
            Destroy(this.gameObject);
        }
        if (transform.position.y < -4.92f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7.12f, 0);
        }
    }

    IEnumerator EnemyAggressionColor()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(.5f);
        _spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(.5f);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_player != null)
        {
            if (other.tag == "Player")
            {
                _player.Damage();
                EnemyDestroyed();
            }

            if (other.tag == "Laser")
            {
                Destroy(other.gameObject);
                _player.AddScore(10);
                EnemyDestroyed();
            }

            if (other.tag == "Bomb")
            {
                _player.AddScore(20);
                Destroy(other.gameObject);
                Instantiate(_bombExplosionPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            }

            if (other.tag == "Bomb_Explosion")
            {
                _player.AddScore(25);
                EnemyDestroyed();
            }
        }
        void EnemyDestroyed()
        {
            if (_isEnemyShieldActive == true)
            {
                _isEnemyShieldActive = false;
                _enemyShieldVisualizer.SetActive(false);
            }
            else
            {
                _enemyCollider.enabled = false;
                _anim.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
                _spawnManager.enemiesKilled++;
                Destroy(this.gameObject, 2.8f);
            }
        }
    }
}


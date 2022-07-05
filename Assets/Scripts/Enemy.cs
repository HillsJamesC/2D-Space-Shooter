using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private Player _player;
    private Animator _anim;
    private Collider2D _enemyCollider;
    private AudioSource _audioSource;
    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    [SerializeField] private GameObject _bombExplosionPrefab;
    private int _randomMovement; //0 = Down, 1 = Wave
    
    void Start()
    {   _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _enemyCollider = GetComponent<Collider2D>();
        _randomMovement = Random.Range(0, 2);

        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        _enemyCollider = GetComponent<Collider2D>();

        if (_enemyCollider == null)
        {
            Debug.LogError("The Collider2D is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        // Using Switch statement to later add more movements
        switch (_randomMovement) 
        {
            case 1:
                transform.Translate(new Vector3(Mathf.Cos(Time.time * 4) * 2, -1, 0) * _speed * Time.deltaTime);
                break;
            //case 2:
            //case 3:
            default:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
        }
        

        if (_enemyCollider.enabled == false && transform.position.y < -4.92f)
        {
            Destroy(this.gameObject);
        }
        else if (transform.position.y < -4.92f)
        {
            float randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 7.12f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player.Damage();
            EnemyDestroyed();
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            EnemyDestroyed();
        }

        if (other.tag == "Bomb")
        {
            if (_player != null)
            {
                _player.AddScore(25);
                Destroy(other.gameObject);
                Instantiate(_bombExplosionPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            }
        }

        if (other.tag == "Bomb_Explosion")
        {
            if (_player != null)
            {
                _player.AddScore(20);
                EnemyDestroyed();
            }

        }

        void EnemyDestroyed()
        {
            _enemyCollider.enabled = false;
            _anim.SetTrigger("OnEnemyDeath");
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
        }
    }
}


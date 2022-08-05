using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private int powerupID; // 0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Ammo, 4 = Health, 5 = Slow Speed
    private float _powerupMagnetSpeed = 6.5f;
    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent <Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);
        if (_player._isPowerupMagnetActive == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _powerupMagnetSpeed * Time.deltaTime);
        }

        if (transform.position.y < -4.92f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();            

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostCollected();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoCollected();
                        break;
                    case 4:
                        player.HealthCollected();
                        break;
                    case 5:
                        player.SlowSpeedActive();
                        break;
                    case 6:
                        player.BombsCollected();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
        if (other.CompareTag("EnemyLaser") && this.gameObject.name != "Slow_Powerup")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}

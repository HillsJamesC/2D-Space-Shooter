using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    public bool isEnemyLaser = false;
    private bool _isEnemyBackShot = false;

    // Update is called once per frame
    void Update()
    {
        if (isEnemyLaser == false || _isEnemyBackShot == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }    

    void MoveUp()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.up);

        if (transform.position.y > 6.93f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < -3.68f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    public void AssignEnemyLaser()
    {
        isEnemyLaser = true;
    }

    public void AssignEnemyBackShot()
    {
        _isEnemyBackShot = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (isEnemyLaser == true || _isEnemyBackShot == true))
        {
            
            if (other.TryGetComponent<Player>(out var player))
            {
                player.Damage();
            }
        }
    }   
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour
{
    bool _closestEnemyNull = true;
    private Rigidbody2D _rigidbody2D;
    private float _moveSpeed = 8f;
    private float _rotateSpeed = 250f;

    private void Start()
    {
        StartCoroutine(DestroyMissle());
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestEnemy();
    }

    private void FixedUpdate()
    {
        if (_closestEnemyNull == true)
        {
            _rigidbody2D.velocity = transform.up * _moveSpeed;
            transform.rotation = Quaternion.identity;
        }
    }


    void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        Enemy closestEnemy = null;
        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();


        foreach (Enemy currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;

            if (distanceToEnemy < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;
            }
        }

        if (closestEnemy != null)
        {
            _closestEnemyNull = false;
            Vector2 targetDirection = (Vector2)closestEnemy.transform.position - _rigidbody2D.position;
            targetDirection.Normalize();
            float rotateAmount = Vector3.Cross(targetDirection, transform.up).z;
            _rigidbody2D.angularVelocity = -rotateAmount * _rotateSpeed;
            _rigidbody2D.velocity = transform.up * _moveSpeed;
        }
        else if (closestEnemy == null)
        {
            _closestEnemyNull = true;
        }
    }

    IEnumerator DestroyMissle()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(this.gameObject);
    }
}

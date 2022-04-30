using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{        
    [SerializeField]
    private float _speed = 3.5f;

        // Start is called before the first frame update
    void Start()
    {        
        transform.position = new Vector3(0, 0, 0);
    }

        // Update is called once per frame
    void Update()
    {
     CalculateMovement();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * _speed * horizontalInput * Time.deltaTime);
        transform.Translate(Vector3.up * _speed * verticalInput * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 2f), 0);

        if (transform.position.x > 11.26f)
        {
            transform.position = new Vector3(-11.26f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.26f)
        {
            transform.position = new Vector3(11.26f, transform.position.y, 0);
        }
    }
}

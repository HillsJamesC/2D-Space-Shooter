using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEBomb : MonoBehaviour
{
    public float _newScaleX = 0;
    public float _scaleTime = 0f;

    void Update()
    {
        if (_scaleTime < 100f)
        {
            _newScaleX += 0.002f;
            transform.localScale = new Vector3(transform.localScale.x + _newScaleX, transform.localScale.y, transform.localScale.z);
            _scaleTime += 1.4f;
        }
        else if (_scaleTime >= 100f)
        {
            Destroy(this.gameObject);
        }

    }    
}

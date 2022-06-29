using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float _timePassed = 0f;

    public IEnumerator Shake (float timeSpan, float strength)
    {
        Vector3 startPos = transform.localPosition;
        
        while (_timePassed < timeSpan)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            transform.localPosition = new Vector3(x, y, startPos.z);
            _timePassed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
        _timePassed = 0f;
    }
}

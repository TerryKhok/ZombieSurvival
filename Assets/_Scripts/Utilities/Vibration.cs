using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
    [SerializeField] private float intensity = 1f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float duration = .5f;

    public void StartVibration(){
        StartCoroutine(Vibrate());
    }

    IEnumerator Vibrate()
    {
        float timePassed = 0;
        while (timePassed < duration)
        {

            transform.localPosition = intensity * new Vector3(
                Mathf.PerlinNoise(speed * Time.time, 1),
                Mathf.PerlinNoise(speed * Time.time, 2),
                0);
            timePassed += Time.deltaTime;

            yield return null;
        }
    }
}

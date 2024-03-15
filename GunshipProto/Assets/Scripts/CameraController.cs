using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private float _shakeTime = 0f;
    private float _shakeIntensity = 1f;

    [SerializeField] private GameObject ship;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Camera>().orthographicSize = Math.Clamp(ship.GetComponent<Ship>().Velocity * 1.5f + 2f, 3f, 8f);

    }

    public void AddShakeTime(float shakeTime)
    {
        _shakeTime += shakeTime;
    }

    public void SetShakeIntensity(float shakeIntensity)
    {
        _shakeIntensity = shakeIntensity;
    }
}

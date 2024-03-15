using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    private float _hitStopTime = 0f;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_hitStopTime > 0f)
        {
            Time.timeScale = 0.002f;
        }

        _hitStopTime -= Time.unscaledDeltaTime;
        if (_hitStopTime < 0f)
        {
            _hitStopTime = 0f;
            Time.timeScale = 1f;

        }
    }



    public void AddHitStopTime(float hitStopTime)
    {
        _hitStopTime += hitStopTime;
    }

}

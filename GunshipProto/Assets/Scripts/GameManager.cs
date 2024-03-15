using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public int _droneCount = 0;

    public int MaxDrones = 20;

    Vector2 point = Vector2.zero;


    private float _spawnTimer = 0;
    private float _pointTimer = 0;

    [SerializeField] GameObject dronePrefab;

    // Start is called before the first frame update
    void Start()
    {
        point = new Vector2(Random.Range(-10f, 10f), Random.Range(-6f, 6f));
    }

    // Update is called once per frame
    void Update()
    {
        if (_droneCount < MaxDrones && _spawnTimer >= _droneCount / 10f)
        {
            GameObject drone = Instantiate(dronePrefab, new Vector3(Random.Range(-10, 10), Random.Range(-6, 6), 0), Quaternion.identity);
            ++_droneCount;
            _spawnTimer = 0f;

        }

        if (_pointTimer >= 15f)
        {
            point = new Vector2(Random.Range(-10f, 10f), Random.Range(-6f, 6f));
            Debug.Log("The new point is: x:" + point.x + ", y:" + point.y);
            _pointTimer = 0f;
        }
        _spawnTimer += Time.deltaTime;
        _pointTimer += Time.deltaTime;


    }

    public void DecrementDroneCount()
    {
        --_droneCount;
    }
}

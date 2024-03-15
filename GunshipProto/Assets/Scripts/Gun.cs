using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour
{

    [SerializeField] private GameObject _bullet;

    [SerializeField] private List<float> _scaleList = new List<float>();
    [SerializeField] private List<int> _damageList = new List<int>();


    [Header("Rate of Fire")]
    //max speed for each type
    [SerializeField] private List<float> _ROFList= new List<float>();
    //max accel for each type
    [SerializeField] private List<float> _ROFAccelList = new List<float>();
    //jerk for each type
    [SerializeField] private List<float> _ROFJerkList = new List<float>();
    //drag value for each type
    //[SerializeField] private List<float> _dragList = new List<float>();




    [SerializeField] private List<float> _bulletSpeedList = new List<float>();
    [SerializeField] private List<float> _bulletLifeList = new List<float>();







    private float _ROFJerk = 0f;
    private float _ROFAcceleration = 0f;
    private float _ROFVelocity = 0f;

    private float _shotTimer = 0f;


    public float ROFJerk => _ROFJerk;

    public float ROFAcceleration => _ROFAcceleration;

    public float ROFVelocity => _ROFVelocity;


    private Ship _shipRef;
    private Drone _droneRef;


    // Start is called before the first frame update
    void Start()
    {
        _shipRef = gameObject.GetComponent<Ship>();
        _droneRef = gameObject.GetComponent<Drone>();
        Assert.IsTrue(_shipRef == null ^ _droneRef == null, "GUN HAS NO PARENT SHIP/DRONE");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            SetJerk(_ROFJerkList[(int)_shipRef.ShipType]);

        }
        else
        {
            SetJerk(-_ROFJerkList[(int)_shipRef.ShipType]);
        }
        //check if can spawn a bullet
        if (_shotTimer >= 1f/ROFVelocity)
        {
            //spawn the bullet
            GameObject bullet = Instantiate(_bullet, gameObject.transform.position, Quaternion.Euler(0, 0, -42f + gameObject.transform.rotation.eulerAngles.z));

            if (_shipRef != null)
            {
                bullet.transform.localScale *= _scaleList[(int)_shipRef.ShipType];

                bullet.GetComponent<Bullet>().damage = _damageList[(int)_shipRef.ShipType];
                bullet.GetComponent<Bullet>().life = _bulletLifeList[(int)_shipRef.ShipType];
                bullet.GetComponent<Bullet>().parent = gameObject;

                //true means come from the player
                bullet.GetComponent<Bullet>().source = true;
                //give the bullet a life timer
                Destroy(bullet, _bulletLifeList[(int)_shipRef.ShipType]);

                //give the bullet velocity
                bullet.GetComponent<Rigidbody2D>().velocity = gameObject.transform.right * _bulletSpeedList[(int)_shipRef.ShipType];
            }
            else if (_droneRef)
            {

            }
            
            _shotTimer = 0f;
        }
        _shotTimer += Time.deltaTime;

        


    }

    void FixedUpdate()
    {
        UpdateROFAcceleration();
        UpdateROF();
    }

    void SetJerk(float jerk)
    {
        _ROFJerk = jerk;
    }

    void UpdateROFAcceleration()
    {
        float dt = Time.fixedDeltaTime;
        //clamp to min max for ship size
        _ROFAcceleration = Math.Clamp(_ROFAcceleration + (_ROFJerk * dt), -_ROFAccelList[(int)_shipRef.ShipType], _ROFAccelList[(int)_shipRef.ShipType]);
        if (ROFVelocity < 0.1f && _ROFAcceleration < 0f)
        {
            _ROFAcceleration = 0f;
        }
    }
    void UpdateROF()
    {
        float dt = Time.fixedDeltaTime;

        //clamp to min max for ship size
        _ROFVelocity = Math.Clamp(_ROFAcceleration + (_ROFAcceleration * dt), 0, _ROFList[(int)_shipRef.ShipType]);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Events;

public class Ship : MonoBehaviour
{
    public enum ShipSize
    {
        Invalid = -1,
        Small = 0,
        Medium,
        Large
    }

    public ShipSize ShipType = ShipSize.Small;


    [Header("Events")]
    [Tooltip("On Hit")]
    [SerializeField] public UnityEvent onHit;
    [Tooltip("On Take Damage(damage)")]
    [SerializeField] public UnityEvent<int> onTakeDamage;
    [SerializeField] public UnityEvent onShoot;
    [SerializeField] public UnityEvent onSpawn;
    [SerializeField] public UnityEvent onTypeChange;
    [SerializeField] public UnityEvent onDead;




    [Header("Ship Sprites")]
    [SerializeField] private List<Sprite> _spriteList = new List<Sprite>();
    [SerializeField] private List<Vector3> _scalesList = new List<Vector3>();



    [Header("Shmovement")]
    //max speed for each type
    [SerializeField] private List<float> _speedList = new List<float>();
    [SerializeField] private List<float> _RotSpeedList = new List<float>();
    //max accel for each type
    [SerializeField] private List<float> _accelList = new List<float>();
    [SerializeField] private List<float> _RotAccelList = new List<float>();
    //jerk for each type
    [SerializeField] private List<float> _jerkList = new List<float>();
    [SerializeField] private List<float> _rotJerkList = new List<float>();

    //drag value for each type
    [SerializeField] private List<float> _dragList = new List<float>();


    private float _jerk = 0f;
    private float _acceleration = 0f;
    private float _velocity = 0f;
    private float _rotJerk = 0f;
    private float _rotAcceleration = 0f;
    private float _rotVelocity = 0f;
    //private Vector2 _drag = new Vector2(1f,1f);


    private int _maxHealth = 10;
    private int _health = 10;
    private GameObject _cameraRef = null;


    public float Jerk => _jerk;

    public float Acceleration => _acceleration;

    public float Velocity => _velocity;

    public float RotJerk => _rotJerk;

    public float RotAcceleration => _rotAcceleration;

    public float RotVelocity => _rotVelocity;

    public int MaxHealth => _maxHealth;

    public int Health => _health;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _cameraRef = GameObject.Find("Main Camera");


        //init to the ship size
        ChangeShipTypes(ShipType);
        onSpawn.AddListener(()=> ChangeShipTypes(ShipType));
    }

    // Update is called once per frame
    void Update()
    {

        //@TODO replace with "can change ship"
        if (true)
        {
            //small ship
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeShipTypes(ShipSize.Small);
            }
            //med ship
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeShipTypes(ShipSize.Medium);

            }
            //big ship
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeShipTypes(ShipSize.Large);
            }

        }

        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            SetJerk(_jerkList[(int)ShipType]);
        }
        //reverse/slow down
        else if (Input.GetKey(KeyCode.S))
        {
            SetJerk(-_jerkList[(int)ShipType]);

        }
        else
        {
            
            if ((Velocity <= 0.5f && Velocity >= -0.5f))
            {
                _acceleration = 0f;
                _jerk = 0f;
                _velocity = 0f;
            }
            else
            {
                if (Velocity > 0.2f) _jerk = -_jerkList[(int)ShipType];
                else if (Velocity < -0.2f) _jerk = _jerkList[(int)ShipType];
            }
        }

        //rotate left
        if (Input.GetKey(KeyCode.A))
        {
            SetRotJerk(_rotJerkList[(int)ShipType]);
        }
        //rotate right
        else if( Input.GetKey(KeyCode.D))
        {
            SetRotJerk(-_rotJerkList[(int)ShipType]);

        }
        else
        {
            if (RotVelocity <= 2f && RotVelocity >= -2f)
            {
                _rotAcceleration = 0f;
                _rotJerk = 0f;
                _rotVelocity = 0f;
            }
            else
            {
                if (RotVelocity > 1f) _rotJerk = -_rotJerkList[(int)ShipType];
                else if (RotVelocity < -1f) _rotJerk = _rotJerkList[(int)ShipType];
            }
        }


        CheckForTeleport();
    }

    void FixedUpdate()
    {
        UpdateRotAccel();
        UpdateRotVelocity();

        UpdateAccel();
        UpdateVelocity();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (bullet.IsDroneBullet())
            {
                other.gameObject.GetComponent<Bullet>().parent.GetComponent<Drone>().onHit.Invoke();
                onTakeDamage.Invoke(bullet.damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth( -damage);
        onTakeDamage.Invoke(damage);
    }


    /// <summary>
    /// Sets the health of the ship
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        _health = health;
    }

    /// <summary>
    /// Adds the input value (positive or negative) the health of the ship
    /// </summary>
    /// <param name="health"></param>
    public void ModifyHealth(int health)
    {
        _health += health;
    }

    /// <summary>
    /// Applys an impulse in the direction of the forward vector of the collided object
    /// </summary>
    /// <param name="knockback"></param>
    public void ApplyKnockback(float knockback)
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector3(1f, 0f, 0f) * knockback, ForceMode2D.Impulse);
    }



    /// <summary>
    /// Updates the ships parameters to the new ship type
    /// </summary>
    /// <param name="size"></param>
    private void ChangeShipTypes(ShipSize size)
    {
        //change size enum
        ShipType = size;
        //change sprite
        GetComponent<SpriteRenderer>().sprite = _spriteList[(int)size];
        //change scale
        transform.localScale = _scalesList[(int)size];

        SetJerk(_jerkList[(int)size]);
    }

    /// <summary>
    /// Sets Jerk Value
    /// </summary>
    /// <param name="jerk"></param>
    void SetJerk(float jerk)
    {
        _jerk = jerk;
    }

    void UpdateAccel( float radian = 0f)
    {
        float dt = Time.fixedDeltaTime;
        //clamp to min max for ship size
        _acceleration = Math.Clamp( _acceleration + (_jerk * dt), -_accelList[(int)ShipType], _accelList[(int)ShipType]);
        //Debug.Log("Acceleration: " + (float)_acceleration);
    }

    void UpdateVelocity()
    {
        float dt = Time.fixedDeltaTime;

        //clamp to min max for ship size
        _velocity = Math.Clamp(_velocity + ( _acceleration * dt), -_speedList[(int)ShipType], _speedList[(int)ShipType]);

        GetComponent<Rigidbody2D>().velocity = transform.right * _velocity /** -_drag*/;
        //Debug.Log("Velocity: " + _velocity);

    }

    /// <summary>
    /// Sets Rotational Jerk Value
    /// </summary>
    /// <param name="jerk"></param>
    void SetRotJerk(float jerk)
    {
        _rotJerk = jerk;
    }

    void UpdateRotAccel(float radian = 0f)
    {
        float dt = Time.fixedDeltaTime;
        //clamp to min max for ship size
        _rotAcceleration = Math.Clamp(_rotAcceleration + (_rotJerk * dt), -_RotAccelList[(int)ShipType], _RotAccelList[(int)ShipType]);
        //Debug.Log("Acceleration: " + (float)_acceleration);
    }

    void UpdateRotVelocity()
    {
        float dt = Time.fixedDeltaTime;

        //clamp to min max for ship size
        _rotVelocity = Math.Clamp(_rotVelocity + (_rotAcceleration * dt), -_RotSpeedList[(int)ShipType], _RotSpeedList[(int)ShipType]);

        GetComponent<Rigidbody2D>().angularVelocity = _rotVelocity /** -_drag*/;
        //Debug.Log("Velocity: " + _velocity);

    }

    /// <summary>
    /// Teleports the owner based on their position
    /// </summary>
    void CheckForTeleport()
    {
        
        float left = -10;
        float right = 10;
        float top = 5;
        float bottom = -5;


        Vector3 vel = GetComponent<Rigidbody2D>().velocity;
        Vector3 pos = transform.position;


        if ((vel.x > 0) && (pos.x > right))
        {
            pos.x = left;
            transform.position = pos;
        }
        else if ((vel.x < 0) && (pos.x < left))
        {
            pos.x = right;
            transform.position = pos;

        }
        else if ((vel.y > 0) && (pos.y > top))
        {
            pos.y = bottom;
            transform.position = pos;

        }
        else if ((vel.y < 0) && (pos.y < bottom))
        {
            pos.y = top;
            transform.position = pos;
        }
    }
}

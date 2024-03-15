using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.Events;

public class Drone : MonoBehaviour
{

    [Header("Events")]
    [Tooltip("On Hit")]
    [SerializeField] public UnityEvent onHit;
    [Tooltip("On Take Damage(damage)")]
    [SerializeField] public UnityEvent<int> onTakeDamage;
    [SerializeField] public UnityEvent onShoot;
    [SerializeField] public UnityEvent onSpawn;
    [SerializeField] public UnityEvent onTypeChange;
    [SerializeField] public UnityEvent onDead;


    [SerializeField] private float _maxJerk = 0f;
    [SerializeField] private float _maxAcceleration = 0f;
    [SerializeField] private float _maxVelocity = 0f;


    private float _jerk = 0f;
    private float _acceleration = 0f;
    private float _velocity = 0f;
    //private Vector2 _drag = new Vector2(1f,1f);


    private int _maxHealth = 10;
    private int _health = 10;
    private GameObject _cameraRef = null;


    public float Jerk => _jerk;

    public float Acceleration => _acceleration;

    public float Velocity => _velocity;

    public int MaxHealth => _maxHealth;

    public int Health => _health;
    // Start is called before the first frame update
    void Start()
    {
        _cameraRef = GameObject.Find("Main Camera");
        onDead.AddListener(()=>Destroy(gameObject, 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateAccel();
        UpdateVelocity();
    }

    /// <summary>
    /// Sets Jerk Value
    /// </summary>
    /// <param name="jerk"></param>
    void SetJerk(float jerk)
    {
        _jerk = jerk;
    }

    void UpdateAccel(float radian = 0f)
    {
        float dt = Time.fixedDeltaTime;
        //clamp to min max for ship size
        _acceleration = Math.Clamp(_acceleration + (_jerk * dt), -_maxAcceleration, _maxAcceleration);
        //Debug.Log("Acceleration: " + (float)_acceleration);
    }

    void UpdateVelocity()
    {
        float dt = Time.fixedDeltaTime;

        //clamp to min max for ship size
        _velocity = Math.Clamp(_velocity + (_acceleration * dt), -_maxVelocity, _maxVelocity);

        GetComponent<Rigidbody2D>().velocity = transform.right * _velocity /** -_drag*/;
        //Debug.Log("Velocity: " + _velocity);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (bullet.IsPlayerBullet())
            {
                other.gameObject.GetComponent<Bullet>().parent.GetComponent<Ship>().onHit.Invoke();
                onTakeDamage.Invoke(bullet.damage);
            }
        }
    }


    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage);
        Debug.Log(gameObject.name + " took " + damage + "damage");
        onTakeDamage.Invoke(damage);

        if (Health <= 0)
        {
            onDead.Invoke();
        }
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
    /// Applys an impulse in the direction of the forward vector of self
    /// </summary>
    /// <param name="knockback"></param>
    public void ApplyKnockback(float knockback)
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector3(1f,0f,0f) * knockback, ForceMode2D.Impulse);
        //apply impulse
    }
}

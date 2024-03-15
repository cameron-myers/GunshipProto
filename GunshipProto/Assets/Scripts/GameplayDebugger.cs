using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayDebugger : MonoBehaviour
{
    public struct DataPack
    {

        DataPack(int _id = -1)
        {
            id = _id;
            ship_size = Ship.ShipSize.Invalid;
            ship_health = -1;
            ship_velocity = 0f;
            ship_acceleration = 0f;
            ship_jerk = 0f;

            ship_rot_velocity = 0f;
            ship_rot_acceleration = 0f;
            ship_rot_jerk = 0f;

            gun_damage = 0f;
            gun_jerk = 0f;
            gun_acceleration = 0f;
            gun_rof = 0f;

        }

        private int id;
        //Ship Data
        private Ship.ShipSize ship_size;
        private int ship_health;
        private float ship_velocity;
        private float ship_acceleration;
        private float ship_jerk;

        private float ship_rot_velocity;
        private float ship_rot_acceleration;
        private float ship_rot_jerk;

        //Gun Data
        private float gun_damage;
        private float gun_jerk;
        private float gun_acceleration;
        private float gun_rof;

        public Ship.ShipSize ShipSize
        {
            get => ship_size;
            set => ship_size = value;
        }

        public int ShipHealth
        {
            get => ship_health;
            set => ship_health = value;
        }

        public float ShipVelocity
        {
            get => ship_velocity;
            set => ship_velocity = value;
        }

        public float ShipAcceleration
        {
            get => ship_acceleration;
            set => ship_acceleration = value;
        }

        public float ShipJerk
        {
            get => ship_jerk;
            set => ship_jerk = value;
        }

        public float ShipRotVelocity
        {
            get => ship_rot_velocity;
            set => ship_rot_velocity = value;
        }

        public float ShipRotAcceleration
        {
            get => ship_rot_acceleration;
            set => ship_rot_acceleration = value;
        }

        public float ShipRotJerk
        {
            get => ship_rot_jerk;
            set => ship_rot_jerk = value;
        }

        public float GunDamage
        {
            get => gun_damage;
            set => gun_damage = value;
        }

        public float GunJerk
        {
            get => gun_jerk;
            set => gun_jerk = value;
        }

        public float GunAcceleration
        {
            get => gun_acceleration;
            set => gun_acceleration = value;
        }

        public float GunRof
        {
            get => gun_rof;
            set => gun_rof = value;
        }
    }

    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();
    private bool toggled = false;

    private DataPack data = new DataPack();
    void Start()
    {

        Debug.Log("Started up logging.");
    }

    void OnEnable()
    {
        //Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        //Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            toggled = !toggled;
        }
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (toggled)
        {
            myLogQueue.Enqueue("[" + type + "] : " + logString);
            if (type == LogType.Exception)
                myLogQueue.Enqueue(stackTrace);
            while (myLogQueue.Count > qsize)
                myLogQueue.Dequeue();
        }
    }

    void OnGUI()
    {
        if (toggled)
        {
            CollectData();
            PresentData();

        }
    }


    void CollectData()
    {
        GameObject player = GameObject.Find("Ship");

        if (player)
        {
            Ship ship = player.GetComponent<Ship>();
            if (ship != null)
            {
                data.ShipSize = ship.ShipType;
                data.ShipHealth = ship.Health;
                data.ShipVelocity = ship.Velocity;
                data.ShipAcceleration = ship.Acceleration;
                data.ShipJerk = ship.Jerk;

                data.ShipRotVelocity = ship.RotVelocity;
                data.ShipRotAcceleration = ship.RotAcceleration;
                data.ShipRotJerk = ship.RotJerk;
            }

            Gun gun = player.GetComponent<Gun>();
            if (gun != null)
            {
                data.GunRof = gun.ROFVelocity;
                data.GunAcceleration = gun.ROFAcceleration;
                data.GunJerk = gun.ROFJerk;
            }
        }
    }

    void PresentData()
    {
        GUILayout.BeginArea(new Rect(20, 0, 300, Screen.height));

        
        //ShipData
        GUILayout.Label("Ship Type: " + System.Enum.GetName(typeof(Ship.ShipSize), data.ShipSize));
        GUILayout.Label("Ship Health: " +  data.ShipHealth);


        GUILayout.Label("Ship Velocity: " + data.ShipVelocity);
        GUILayout.Label("Ship Acceleration: " + data.ShipAcceleration);
        GUILayout.Label("Ship Jerk: " + data.ShipJerk);

        GUILayout.Label("Ship ROT Velocity: " + data.ShipRotVelocity);

        GUILayout.Label("Ship ROT Acceleration: " + data.ShipRotAcceleration);
        GUILayout.Label("Ship ROT Jerk: " + data.ShipRotJerk);

        GUILayout.Label("Gun ROFVelocity: " + data.GunRof);
        GUILayout.Label("Gun ROFAcceleration: " + data.GunAcceleration);
        GUILayout.Label("Gun ROFJerk: " + data.GunJerk);


        GUILayout.EndArea();
    }
}
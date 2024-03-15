using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector]public int damage = 0;
    [HideInInspector]public float life = 0f;
    [HideInInspector]public bool source = false;
    [HideInInspector]public GameObject parent = null;

    void Start()
    {
        
    }

    public bool IsPlayerBullet()
    {
        return source;
    }

    public bool IsDroneBullet()
    {
        return !source;
    }

}

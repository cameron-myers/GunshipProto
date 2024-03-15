using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameObject.Find("Ship").GetComponent<Ship>().ModifyHealth(5);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GameObject.Find("Ship").GetComponent<Ship>().ModifyHealth(-5);
            }

        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    private bool collected;

    public int healAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter( Collider other )
    {
        if (other.tag == "Player" && !collected)
        {
            PlayerHealthController.instance.HealPlayer( healAmount );

            Destroy( gameObject );

            collected = true;
        }
    }
}

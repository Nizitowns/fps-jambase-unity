﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public float waitAfterDying = 2f;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor in the centre of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDied()
    {
        StartCoroutine( PlayerDiedCo() );
        // SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public IEnumerator PlayerDiedCo()
    {
        yield return new WaitForSeconds( waitAfterDying );

        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}
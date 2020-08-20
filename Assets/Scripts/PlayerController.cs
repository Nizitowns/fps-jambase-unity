using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed, gravityModifier, jumpPower, runSpeed = 12;
    public CharacterController charCon;

    private Vector3 moveInput;

    public Transform camTrans;

    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    public Animator anim;

    //public GameObject bullet;
    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public int currentGun;


    private void Awake()
    {
        instance = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        #region Player Movement
        //moveInput.x = Input.GetAxis( "Horizontal" ) * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis( "Vertical" ) * moveSpeed * Time.deltaTime;

        // Store y velocity (part of gravity)
        float yStore = moveInput.y;

        // Player Movement (Input)
        Vector3 vertMove = transform.forward * Input.GetAxis( "Vertical" );
        Vector3 horiMove = transform.right * Input.GetAxis( "Horizontal" );
        moveInput = horiMove + vertMove;
        moveInput.Normalize();

        // Run or Walk
        if (Input.GetButton("Fire3"))
        {
            moveInput = moveInput * runSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }
        

        // Gravity Part
        moveInput.y = yStore;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
        if (charCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        // Handle Jumping & Double Jumping needs fixing as now is working cause of VSync hack 
        // ToDo Remove VSync and solve the issue properly
        canJump = Physics.OverlapSphere( groundCheckPoint.position, .25f, whatIsGround ).Length > 0;

        if (canJump)
        {
            canDoubleJump = false;
        }

        if (Input.GetButtonDown( "Jump" ) && canJump)
        {
            moveInput.y = jumpPower;

            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetButtonDown( "Jump" ))
        {
            moveInput.y = jumpPower;

            canDoubleJump = false;
        }


        charCon.Move( moveInput * Time.deltaTime );
        #endregion


        #region Control The Camera View (Mouse Input Related)
        Vector2 mouseInput = new Vector2( Input.GetAxisRaw( "Mouse X" ),
            Input.GetAxisRaw( "Mouse Y" ) ) * mouseSensitivity;

        if (invertX)
        {
            mouseInput.x = -mouseInput.x;
        }

        if (invertY)
        {
            mouseInput.y = -mouseInput.y;
        }

        transform.rotation = Quaternion.Euler( transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z );

        camTrans.rotation = Quaternion.Euler( camTrans.rotation.eulerAngles +
            new Vector3( -mouseInput.y, 0f, 0f ) );
        #endregion


        // Handle Shooting
        // Single Shots
        if (Input.GetButtonDown("Fire1") && activeGun.fireCounter <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
            {
                if (Vector3.Distance(camTrans.position, hit.point) > 2.0f)
                {
                    firePoint.LookAt( hit.point );
                }
            }
            else
            {
                firePoint.LookAt( camTrans.position + (camTrans.forward * 30f) );
            }

            //Instantiate( bullet, firePoint.position, firePoint.rotation );
            FireShot();
        }

        // Repeating Shots
        if (Input.GetButton("Fire1") && activeGun.canAutoFire)
        {
            if (activeGun.fireCounter <= 0)
            {
                FireShot();
            }
        }

        // Needs update to become generic for inputs (add to input manager)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchGun();
        }

        // Handle player animations
        anim.SetFloat( "moveSpeed", moveInput.magnitude );
        anim.SetBool( "onGround", canJump );
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate( activeGun.bullet, firePoint.position, firePoint.rotation );

            activeGun.fireCounter = activeGun.fireRate;

            UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
        }

    }

    public void SwitchGun()
    {
        activeGun.gameObject.SetActive( false );

        currentGun++;

        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive( true );

        UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
    }
}

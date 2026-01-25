using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class playerController : MonoBehaviour
{
    //singleton stuff, makes sure there's only one player
    public static playerController instance;
    
    //references for character control and camera stuff
    public Transform orientation;
    public Transform camPosition;
    public GameObject playerCam;
    public CinemachineCamera cinemachineCamera;
    
    //references for rigidbody for physics stuff
    public Transform capsuleHitbox;
    public Rigidbody capsuleHitboxRB;

    //values for movement physics
    public float acceleration;
    public float strafeAcceleration;
    public float turnSpeed;
    public float jumpForce;
    
    public float boostAcceleration;

    public float hoverHeight;
    public float hoverStrength;
    public float springDampener;

    //variables for ground detection
    public bool grounded = true;
    public float groundedBuffer = 1f;
    public static float coyoteTime = 0.5f;
    
    //variables for boosting
    private float boostResource = 100f;
    public bool isBoosting = false;
    private float speed = 0f;

    //values and references for UI stuff
    public GameObject boostMeterObject;
    public Slider boostMeter;
    public TextMeshProUGUI speedometerText;
    [SerializeField] private Image boostMeterColour;
    private float boostMeterHue = 0f;
    public TextMeshProUGUI boostText;

    [SerializeField] private GameObject trickText;
    private float trickCooldown = 0.5f;

    public Image boostVFX;

    private void Awake()
    {
        //singleton stuff
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    { 
        //locks and hides mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //assigns the camera, its position and the cinemachine component
        playerCam = GameObject.Find("Player Camera");
        camPosition = playerCam.transform;
        cinemachineCamera = playerCam.GetComponent<CinemachineCamera>();
        
        //assigns the player hitbox position and rigidbody
        capsuleHitbox = transform.Find("capsuleHitbox").transform;
        capsuleHitboxRB = capsuleHitbox.GetComponent<Rigidbody>();
    }

    void groundedCountdown()
    {
        //counts down constantly, as long as the buffer is above 0 grounded is true
        groundedBuffer = (groundedBuffer - Time.deltaTime);
        grounded = groundedBuffer >= 0f;
    }

    void trickCountdown()
    {
        trickCooldown = (trickCooldown - Time.deltaTime);
        if (trickCooldown <= 0f)
        {
            trickText.SetActive(false);
        }
    }

    void jump()
    {
        //gets the input, performs a jump if the player is grounded
        bool jumpInput = Input.GetButtonDown("Jump");
        if (jumpInput)
        {
            if (grounded)
            {
                capsuleHitboxRB.AddForce(capsuleHitbox.up * jumpForce, ForceMode.Impulse);
            }
            else if (trickCooldown <= 0f)
            {
                airTrick();
            }
        } 
    }

    void airTrick()
    {
        GameManager.instance.addComboChain();
        trickCooldown = 0.5f;
        trickText.SetActive(true);

        addBoostResource(10f);
    }

    public void addBoostResource(float amount)
    {
        //adds boost resource but never enough to exceed the maximum
        if (boostResource <= (100 - amount))
        {
            boostResource += amount;
        }
        else
        {
            boostResource = 100f;
        }
    }

    void boost()
    {
        Color boostVFXColor = Color.white;
        //gets the input, applies a constant force if the key is held, also smoothly adjusts the FOV of the camera
        //also smoothly adjusts the camera FOV back, but faster
        //also increases/decreases the alpha value of the boost VFX
        bool boostInput = Input.GetKey(KeyCode.LeftShift);
        if (boostInput && boostResource >= 0f)
        {
            isBoosting = true;
            capsuleHitboxRB.AddForce(capsuleHitbox.forward * boostAcceleration, ForceMode.Acceleration);
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, 130f, Time.deltaTime * 3);
            boostResource -= Time.deltaTime * 10 ;
            
            boostVFXColor.a = Mathf.Lerp(boostVFX.color.a, 1f, Time.deltaTime * 3f);
            boostVFX.color = boostVFXColor;
        }
        else
        {
            isBoosting = false;
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, 90f, Time.deltaTime * 4);
            boostVFXColor.a = Mathf.Lerp(boostVFX.color.a, 0f, Time.deltaTime * 4f);
            boostVFX.color = boostVFXColor;
        }
    }

    void rotatePlayerToCamera()
    {
        //creates a new vector that represents the direction from the camera to the player, ignoring the Y axis,
        //uses the empty orientation gameobject's transform's forward vector to store this direction,
        //then smoothly adjusts the forward vector of the player's transform to rotate the player.
        //currently is slow after building and running on slower machines needs fixed
        Vector3 camDirection = capsuleHitbox.position - new Vector3(camPosition.position.x, capsuleHitbox.position.y, camPosition.position.z);
        orientation.forward = camDirection.normalized;
        capsuleHitbox.forward = Vector3.Slerp(capsuleHitbox.forward, orientation.forward, turnSpeed * Time.fixedDeltaTime);
    }
    
    void Update()
    {
        //updates all the things
        groundedCountdown();
        trickCountdown();
        jump();
        boost();
        rotatePlayerToCamera();
        updateBoostUI(capsuleHitboxRB.linearVelocity.magnitude);
    }

    void movement()
    {
        //gets the inputs for the 2 axis, then applies acceleration when their keys are held
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        
        if (verticalInput > 0)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.forward * acceleration, ForceMode.Acceleration);
        }

        if (horizontalInput != 0)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.right * strafeAcceleration * horizontalInput, ForceMode.Acceleration);
        } 
    }

    void hoveringAndGroundDetection()
    {
        //sends a raycast down, uses it to tell how far away the ground is, accelerates the board up or down following a dampening ratio tied to height
        //needs to be adjusted with object layers in the future
        RaycastHit groundHit;
        if (Physics.Raycast(capsuleHitbox.position, transform.TransformDirection(Vector3.down), out groundHit, 3f,LayerMask.GetMask("Default")))
        { 
            Vector3 velocity = capsuleHitboxRB.linearVelocity;
            float downVelocity = Vector3.Dot(Vector3.down, velocity);
            
            float hoverDifference = groundHit.distance - hoverHeight;
            float springForce = (hoverDifference * hoverStrength) - (downVelocity * springDampener);
            
            capsuleHitboxRB.AddForce(Vector3.down * springForce, ForceMode.Acceleration);
            
            //resets the grounded buffer countdown
            //add input buffering back up in the jump method maybe?
            groundedBuffer = coyoteTime;
        }
    }
    
    void FixedUpdate()
    {
        //has to be here for physics reasons
        movement();
        hoveringAndGroundDetection();
    }

    private void updateBoostUI(float speed)
    {
        //updates the speedometer in the correct format
        speedometerText.text = String.Format("{0:000}", speed) + " M/S";

        //updates the boost meter
        boostMeter.value = boostResource;
        
        //changes the boost meter's colour to make it RGB like the boost VFX
        boostMeterHue = (boostMeterHue + 0.001f) % 1f;

        Color fromHSV = Color.HSVToRGB(boostMeterHue, 1f, 1f);
        
        if (boostResource <= 0)
        {
            //makes the boost meter red if it's empty
            boostMeterColour.color = Color.red;
        }
        else
        {
            boostMeterColour.color = fromHSV;
        }
    }
}

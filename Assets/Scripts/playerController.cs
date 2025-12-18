using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class playerController : MonoBehaviour
{
    public Transform orientation;
    public Transform camPosition;
    public GameObject playerCam;
    public CinemachineCamera cinemachineCamera;
    
    public Transform capsuleHitbox;
    public Rigidbody capsuleHitboxRB;

    public float acceleration;
    public float strafeAcceleration;
    public float turnSpeed;
    public float jumpForce;
    
    public float boostAcceleration;

    public float hoverHeight;
    public float hoverStrength;
    public float springDampener;

    public bool grounded = true;
    public float groundedBuffer = 1f;
    public static float coyoteTime = 0.5f;
    
    private float boostResource = 100f;
    public bool isBoosting = false;
    private float speed = 0f;

    [SerializeField] private Slider boostMeter;
    [SerializeField] private TextMeshProUGUI speedometerText;

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

    void jump()
    {
        //gets the input, performs a jump if the player is grounded
        bool jumpInput = Input.GetButtonDown("Jump");
        if (jumpInput  && grounded)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.up * jumpForce, ForceMode.Impulse);
        } 
    }

    void boost()
    {
        //gets the input, applies a constant force if the key is held, also smoothly adjusts the FOV of the camera
        //also smoothly adjusts the camera FOV back, but faster
        bool boostInput = Input.GetKey(KeyCode.LeftShift);
        if (boostInput)
        {
            isBoosting = true;
            capsuleHitboxRB.AddForce(capsuleHitbox.forward * boostAcceleration, ForceMode.Acceleration);
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, 130f, Time.deltaTime * 3);
        }
        else
        {
            isBoosting = false;
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, 90f, Time.deltaTime * 4);
        }
    }

    void rotatePlayerToCamera()
    {
        //creates a new vector that represents the direction from the camera to the player, ignoring the Y axis,
        //uses the empty orientation gameobject's transform's forward vectore to store this direction,
        //then smoothly adjusts the forward vector of the player's transform to rotate the player.
        //currently is slow after building and running on slower machines needs fixed
        Vector3 camDirection = capsuleHitbox.position - new Vector3(camPosition.position.x, capsuleHitbox.position.y, camPosition.position.z);
        orientation.forward = camDirection.normalized;
        capsuleHitbox.forward = Vector3.Slerp(capsuleHitbox.forward, orientation.forward, turnSpeed * Time.fixedDeltaTime);
    }
    
    void Update()
    {
        groundedCountdown();
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
        movement();
        hoveringAndGroundDetection();
    }

    private void updateBoostUI(float speed)
    {
        speedometerText.text = String.Format("{0:000}", speed) + " M/S";
        
        boostMeter.value = boostResource;
    }
}

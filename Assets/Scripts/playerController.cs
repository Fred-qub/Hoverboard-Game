using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

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
    public float groundedBuffer;
    public static float coyoteTime = 0.5f;

    void Start()
    { 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        playerCam = GameObject.Find("Player Camera");
        camPosition = playerCam.transform;
        cinemachineCamera = playerCam.GetComponent<CinemachineCamera>();
        
        capsuleHitbox = transform.Find("capsuleHitbox").transform;
        capsuleHitboxRB = capsuleHitbox.GetComponent<Rigidbody>();  
    }
    
    void Update()
    {
        groundedBuffer = (groundedBuffer - Time.deltaTime);

        grounded = groundedBuffer >= 0f;
        
        Vector3 camDirection = capsuleHitbox.position - new Vector3(camPosition.position.x, capsuleHitbox.position.y, camPosition.position.z);
        orientation.forward = camDirection.normalized;
        
        //capsuleHitbox.forward = Vector3.Slerp(capsuleHitbox.forward, orientation.forward, turnSpeed * Time.fixedDeltaTime);
        capsuleHitbox.forward = orientation.forward;
        
        bool jumpInput = Input.GetButtonDown("Jump");
        if (jumpInput  && grounded)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.up * jumpForce, ForceMode.Impulse);
        }
        
        bool boostInput = Input.GetKey(KeyCode.LeftShift);
        if (boostInput)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.forward * boostAcceleration, ForceMode.Acceleration);
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, 110f, Time.deltaTime);

        }
        else
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, 90f, Time.deltaTime * 2);
        }
    }

    void FixedUpdate()
    {
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
        
        RaycastHit groundHit;
        
        if (Physics.Raycast(capsuleHitbox.position, transform.TransformDirection(Vector3.down), out groundHit, 3f,LayerMask.GetMask("Default")))
        { 
            Vector3 velocity = capsuleHitboxRB.linearVelocity;
            float downVelocity = Vector3.Dot(Vector3.down, velocity);
            
            float hoverDifference = groundHit.distance - hoverHeight;
            float springForce = (hoverDifference * hoverStrength) - (downVelocity * springDampener);
            
            capsuleHitboxRB.AddForce(Vector3.down * springForce, ForceMode.Acceleration);
            groundedBuffer = coyoteTime;
        }
    }
}

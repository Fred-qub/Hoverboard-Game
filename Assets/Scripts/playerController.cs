using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public Transform orientation;
    public Transform cam;
    
    public Transform capsuleHitbox;
    public Rigidbody capsuleHitboxRB;

    public float acceleration;
    public float strafeAcceleration;
    public float turnSpeed;
    public float jumpForce;

    public float hoverHeight;
    public float hoverStrength;
    public float springDampener;

    void Start()
    { 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        cam = GameObject.Find("Player Camera").transform;
        
        capsuleHitbox = transform.Find("capsuleHitbox").transform;
        capsuleHitboxRB = capsuleHitbox.GetComponent<Rigidbody>();  
    }
    
    void Update()
    {
        Vector3 camDirection = capsuleHitbox.position - new Vector3(cam.position.x, capsuleHitbox.position.y, cam.position.z);
        orientation.forward = camDirection.normalized;
        
        capsuleHitbox.forward = Vector3.Lerp(capsuleHitbox.forward, orientation.forward, turnSpeed * Time.fixedDeltaTime);
        
        bool jumpInput = Input.GetButtonDown("Jump");
        if (jumpInput)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (capsuleHitbox.forward != orientation.forward)
        {
            //capsuleHitboxRB.AddTorque(transform.up * turnSpeed, ForceMode.Acceleration);
        }
        
        
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        
        if (verticalInput > 0)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.forward * acceleration, ForceMode.Acceleration);
        }

        if (horizontalInput != 0)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.right * strafeAcceleration * horizontalInput, ForceMode.Acceleration);
            //capsuleHitboxRB.AddTorque(transform.up * turnSpeed * horizontalInput, ForceMode.Acceleration);
        }
        
        RaycastHit groundHit;
        
        if (Physics.Raycast(capsuleHitbox.position, transform.TransformDirection(Vector3.down), out groundHit, 3f,LayerMask.GetMask("Default")))
        { 
            Debug.DrawRay(capsuleHitbox.position, transform.TransformDirection(Vector3.down) * groundHit.distance, Color.yellow); 
            Debug.Log("Did Hit");
            
            Vector3 velocity = capsuleHitboxRB.linearVelocity;
            float downVelocity = Vector3.Dot(Vector3.down, velocity);
            
            float hoverDifference = groundHit.distance - hoverHeight;
            float springForce = (hoverDifference * hoverStrength) - (downVelocity * springDampener);
            
            
            capsuleHitboxRB.AddForce(Vector3.down * springForce, ForceMode.Acceleration);
        }
        else
        { 
            Debug.DrawRay(capsuleHitbox.position, transform.TransformDirection(Vector3.down) * 1000, Color.white); 
            Debug.Log("Did not Hit");
            //capsuleHitboxRB.AddForce(Physics.gravity * 5, ForceMode.Acceleration);
        }
    }

    void spring()
    {
        
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public Transform orientation;
    public Transform cam;
    
    public Transform capsuleHitbox;
    public Rigidbody capsuleHitboxRB;

    public float acceleration;
    public float turnSpeed;
    
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
        
        capsuleHitbox.forward = Vector3.Slerp(capsuleHitbox.forward, orientation.forward, turnSpeed * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        float verticalInput = Input.GetAxis("Vertical");
        
        if (verticalInput > 0)
        {
            capsuleHitboxRB.AddForce(capsuleHitbox.forward * acceleration, ForceMode.Acceleration);
        }
    }
}

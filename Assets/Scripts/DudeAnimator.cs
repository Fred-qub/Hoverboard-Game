using UnityEngine;


public class DudeAnimator : MonoBehaviour
{
    public Transform waist;
    public Rigidbody capsuleHitboxRB;
    public float Yoffset = -0.005f;
    public float scale = 70f;
    public float minVelocityRequirement = 10f;

    // divide velocity by 70 to scale it down (player normally doesn't move up or down faster than 70, so this makes it around -1 to 1.
    //target position = vector3 zero + new vector (0f, 0f, Yoffset)
    //goes in z because waist has Y and Z flipped cus of blender
    //take an object's local position and lerp it between its current local position and its target position

    private float scaledYVelocity;
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = waist.localPosition;
    }

    void LateUpdate()
    {
        scaledYVelocity = capsuleHitboxRB.linearVelocity.y / scale;
        //Debug.Log("scaled:" + scaledYVelocity);
        //Debug.Log("y velocity" + capsuleHitboxRB.linearVelocity.y);

        if (capsuleHitboxRB.linearVelocity.y < 0)
        {
            scaledYVelocity = -scaledYVelocity;
            targetPosition = new Vector3(initialPosition.x, initialPosition.y, Yoffset); 
        }
        else if (capsuleHitboxRB.linearVelocity.y > 0)
        {
            targetPosition = new Vector3(initialPosition.x, initialPosition.y, -Yoffset);
        }
        
        waist.localPosition = Vector3.Lerp(waist.localPosition, targetPosition, Mathf.Clamp01(scaledYVelocity));
        
        Debug.Log("Target Position: " + targetPosition + "Current Position: " + waist.localPosition + "Scaled Velocity: " + scaledYVelocity);
        
    }
}

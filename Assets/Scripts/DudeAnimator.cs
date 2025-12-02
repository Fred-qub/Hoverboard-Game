using UnityEngine;

public class DudeAnimator : MonoBehaviour
{
    public Transform dudeThighL;
    public Transform dudeThighR;
    public Rigidbody capsuleHitboxRB;
    public float Yoffset = 0f;
    public float scale = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

       // float scaledYVelocity = capsuleHitboxRB.linearVelocity.y * scale;
        //dudeThighL.position = Vector3.Lerp(capsuleHitboxRB.position, new Vector3(capsuleHitboxRB.position.x, scaledYVelocity, capsuleHitboxRB.position.z), Time.deltaTime);
        //dudeThighR.position = dudeThighL.position;
    }
}

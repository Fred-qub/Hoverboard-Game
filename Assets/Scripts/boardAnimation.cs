using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class boardAnimation : MonoBehaviour
{
    public Transform board;
    public Rigidbody capsuleHitboxRB;
    public TrailRenderer trail;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        board = GameObject.Find("board").transform;
        capsuleHitboxRB = GameObject.Find("capsuleHitbox").GetComponent<Rigidbody>();
        trail = GameObject.Find("trailPoint").GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion velRot = Quaternion.Euler(capsuleHitboxRB.linearVelocity.normalized);
        //board.localRotation = Quaternion.Slerp(board.localRotation, velRot, 0.1f * Time.deltaTime);
        trail.emitting = (capsuleHitboxRB.linearVelocity.magnitude >= 100f);

    }
}

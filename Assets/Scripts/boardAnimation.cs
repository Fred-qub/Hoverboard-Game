using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class boardAnimation : MonoBehaviour
{
    public Transform board;
    public Rigidbody capsuleHitboxRB;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        board = GameObject.Find("board").transform;
        capsuleHitboxRB = GameObject.Find("capsuleHitbox").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Takes the current 
        Quaternion velRot = Quaternion.Euler(capsuleHitboxRB.linearVelocity.normalized);
        board.localRotation = Quaternion.Slerp(board.localRotation, velRot, 0.1f * Time.deltaTime);
    }
}

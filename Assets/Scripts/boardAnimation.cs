using UnityEngine;

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
        //boardPivot.rotation = Quaternion.AngleAxis() capsuleHitboxRB.linearVelocity.magnitude;
    }
}

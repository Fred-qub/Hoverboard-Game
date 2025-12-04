using UnityEngine;


public class DudeAnimator : MonoBehaviour
{
    public Transform waist;
    public Rigidbody capsuleHitboxRB;
    public float waistYoffset = -0.005f;
    public float IKPointOffset = 0.1f;
    public float scale = 70f;
    public Transform leftFootIKPoint;
    public Transform rightFootIKPoint;
    
    private float scaledYVelocity;
    private Vector3 targetWaistPosition;
    private Vector3 initialWaistPosition;
    private Vector3 initialLeftFootIKPosition;
    private Vector3 initialRightFootIKPosition;
    private Vector3 targetLeftFootIKPosition;
    private Vector3 targetRightFootIKPosition;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialWaistPosition = waist.localPosition;
        initialLeftFootIKPosition = leftFootIKPoint.localPosition;
        initialRightFootIKPosition = rightFootIKPoint.localPosition;
    }

    void LateUpdate()
    {
        scaledYVelocity = capsuleHitboxRB.linearVelocity.y / scale;

        if (capsuleHitboxRB.linearVelocity.y < 0)
        {
            scaledYVelocity = -scaledYVelocity;
            targetWaistPosition = new Vector3(initialWaistPosition.x, initialWaistPosition.y, waistYoffset);
            targetLeftFootIKPosition = new Vector3(initialLeftFootIKPosition.x, initialLeftFootIKPosition.y-IKPointOffset, initialLeftFootIKPosition.z);
            targetRightFootIKPosition = new Vector3(initialRightFootIKPosition.x, initialRightFootIKPosition.y-IKPointOffset, initialRightFootIKPosition.z);

        }
        else if (capsuleHitboxRB.linearVelocity.y > 0)
        {
            targetWaistPosition = new Vector3(initialWaistPosition.x, initialWaistPosition.y, -waistYoffset);
            targetLeftFootIKPosition = new Vector3(initialLeftFootIKPosition.x, initialLeftFootIKPosition.y + IKPointOffset, initialLeftFootIKPosition.z);
            targetRightFootIKPosition = new Vector3(initialRightFootIKPosition.x, initialRightFootIKPosition.y + IKPointOffset, initialRightFootIKPosition.z);
        }
        waist.localPosition = Vector3.Lerp(waist.localPosition, targetWaistPosition, Mathf.Clamp01(scaledYVelocity));
        leftFootIKPoint.localPosition = Vector3.Lerp(leftFootIKPoint.localPosition, targetLeftFootIKPosition, Mathf.Clamp01(scaledYVelocity)*0.2f);
        rightFootIKPoint.localPosition = Vector3.Lerp(rightFootIKPoint.localPosition, targetRightFootIKPosition, Mathf.Clamp01(scaledYVelocity)*0.2f);
    }
}



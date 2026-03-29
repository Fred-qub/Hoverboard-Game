using UnityEngine;

public class boardAnimation : MonoBehaviour
{
    //right now this script just turns the boost trail on/off, but trick animations would likely be implemented here
    public TrailRenderer trail;
    
    void Start()
    {
        trail = GameObject.Find("trailPoint").GetComponent<TrailRenderer>();
    }
    
    void Update()
    {
        
        trail.emitting = (playerController.instance.isBoosting);

    }
}

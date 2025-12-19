using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class boardAnimation : MonoBehaviour
{
    public TrailRenderer trail;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trail = GameObject.Find("trailPoint").GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        trail.emitting = (playerController.instance.isBoosting);

    }
}

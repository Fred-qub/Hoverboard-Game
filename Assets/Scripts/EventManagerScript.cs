using UnityEngine;
using UnityEngine.Events;
using System;


public class EventManagerScript : MonoBehaviour
{
    public Events[]  eventList;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class Events
{
    public string eventName;
    public UnityEvent anEvent;
}

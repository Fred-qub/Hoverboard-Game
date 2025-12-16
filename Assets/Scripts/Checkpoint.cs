using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the player hits a checkpoint, call the checkpoint passed function over in the game manager
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Checkpoint " + checkpointIndex + " passed");
            GameManager.instance.CheckpointPassed(checkpointIndex);
            
        }
    }
}

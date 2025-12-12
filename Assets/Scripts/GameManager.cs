using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private int lastCheckpointIndex = -1;
    [SerializeField] private Checkpoint[] checkpoints;
    
    private bool raceStarted = false;
    private bool raceFinished = false;

    private int currentLap = 1;
    [SerializeField] private int totalLaps = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void StartRace()
    {
        raceStarted = true;
        raceFinished = false;
        Debug.Log("Race started");
    }

    private void EndRace()
    {
        raceStarted = false;
        raceFinished = true;
        Debug.Log("Race finished");
    }
    
    private void OnLapFinished()
    {
        currentLap++;
        Debug.Log("Lap " + currentLap + "/" + totalLaps);
        if (currentLap > totalLaps)
        {
            EndRace();
        }
    }

    private void UpdateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == 0)
        {
            if (!raceStarted)
            {
                StartRace();
            }
            else if (lastCheckpointIndex == checkpoints.Length - 1)
            {
                Debug.Log("Lap completed");
                OnLapFinished();
            }
        }
        
        lastCheckpointIndex = checkpointIndex;
        Debug.Log("The last checkpoint is now " + lastCheckpointIndex);
    }

    public void CheckpointPassed(int checkpointIndex)
    {
        if ((!raceStarted && checkpointIndex != 0) || raceFinished)
        {
            Debug.Log("Last checkpoint was " + lastCheckpointIndex + ", so this is wrong");
            return;
        }

        if (checkpointIndex == lastCheckpointIndex + 1)
        {
            Debug.Log("Last checkpoint was " + lastCheckpointIndex + ", so this is correct");
            UpdateCheckpoint(checkpointIndex);
        }
    }
}

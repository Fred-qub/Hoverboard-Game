using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Settings")]
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private int totalLaps = 3;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI totalRaceTimeText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI currentLapTimeText;
    [SerializeField] private TextMeshProUGUI prevLapTimeDifferenceText;
    [SerializeField] private TextMeshProUGUI finishText;
    
    private int lastCheckpointIndex = -1;
    private int currentLap = 1;
    
    private bool raceStarted = false;
    private bool raceFinished = false;
    
    private float currentLapTime = 0f;
    private float totalRaceTime = 0f;
    private float prevLapTime = 0f;
    private float lapTimeDifference = 0f;

    private float UIBuffer = 0f;

    private void Update()
    {
        if (raceStarted)
        {
            UpdateTimers();
        }
        
        UpdateTimerUI();
    }
    
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
        lapTimeDifference = currentLapTime - prevLapTime;

        if (lapTimeDifference <= 0)
        {
            prevLapTimeDifferenceText.color = Color.green;
            prevLapTimeDifferenceText.text = "\t        -" + FormatTime(-lapTimeDifference);
        }
        else
        {
            prevLapTimeDifferenceText.color = Color.red;
            prevLapTimeDifferenceText.text = "\t        +" + FormatTime(lapTimeDifference);
        }
        
        UIBuffer = 3f;
        currentLap++;
        prevLapTime = currentLapTime;
        
        Debug.Log("Lap " + currentLap + "/" + totalLaps);
        if (currentLap > totalLaps)
        {
            EndRace();
        }
        else
        {
            currentLapTime = 0f;
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
            return;
        }

        if ((checkpointIndex == lastCheckpointIndex + 1) || (checkpointIndex == 0 && lastCheckpointIndex == checkpoints.Length - 1))
        {
            Debug.Log("Last checkpoint was " + lastCheckpointIndex + ", so this is correct");
            UpdateCheckpoint(checkpointIndex);
        }
        else
        {
            Debug.Log("Last checkpoint was " + lastCheckpointIndex + ", so this is wrong");
        }
    }

    private void UpdateTimers()
    {
        currentLapTime += Time.deltaTime;
        totalRaceTime += Time.deltaTime;
        UIBuffer = Mathf.Lerp(UIBuffer, 0f, Time.deltaTime);
        
    }
    
    private void UpdateTimerUI()
    {
        currentLapTimeText.text = "Lap Time: \t" + FormatTime(currentLapTime);
        totalRaceTimeText.text = "Time: \t" + FormatTime(totalRaceTime);
        lapText.text = "Lap: " + currentLap + "/" + totalLaps;
        prevLapTimeDifferenceText.alpha = UIBuffer * 85f;

        if (raceFinished)
        {
            currentLapTimeText.enabled = false;
            totalRaceTimeText.enabled = false;
            lapText.enabled = false;
            finishText.enabled = true;
            prevLapTimeDifferenceText.enabled = false;
        }
    }

    private string FormatTime(float time)
    {
        if (float.IsInfinity(time) || float.IsNaN(time)) return "--:--.---";
        
        int minutes = (int)time / 60;
        float seconds = time % 60;
        
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }
}

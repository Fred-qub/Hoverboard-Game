using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Settings")]
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private int totalLaps = 3;
    [SerializeField] private GameObject[] boostCapsules;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI totalRaceTimeText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI currentLapTimeText;
    [SerializeField] private TextMeshProUGUI prevLapTimeDifferenceText;
    [SerializeField] private TextMeshProUGUI finishText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    
    [SerializeField] private GameObject comboBufferSliderObject;
    private Slider comboSlider;
    [SerializeField] private TextMeshProUGUI comboBufferText;
    [SerializeField] private TextMeshProUGUI comboChainText;
    [SerializeField] private TextMeshProUGUI trickScoreIncreaseText;

    
    private int score = 0;
    private int comboMultiplier = 1;
    private int comboChain = 0;
    private int comboGaugeInt = 0;
    
    
    private int lastCheckpointIndex = -1;
    private int currentLap = 1;
    
    private bool raceStarted = false;
    private bool raceFinished = false;
    
    private float currentLapTime = 0f;
    private float totalRaceTime = 0f;
    private float prevLapTime = 0f;
    private float lapTimeDifference = 0f;

    private float prevLapTimeDiffTextBuffer = 0f;
    private float comboBuffer = 0f;
    private float trickScoreIncreaseTextBuffer = 0f;

    private void Start()
    {
        comboSlider = comboBufferSliderObject.GetComponent<Slider>();
    }
    
    private void Update()
    {
        if (raceStarted)
        {
            UpdateTimers();
        }
        
        UpdateCombo();
        UpdateUI();
    }
    
    private void Awake()
    {
        //singleton stuff
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
        //starts the race, but makes sure it isn't finished either
        raceStarted = true;
        raceFinished = false;
        //Debug.Log("Race started");
    }

    private void EndRace()
    {
        //This is probably doesn't need an explanation
        raceStarted = false;
        raceFinished = true;
        //Debug.Log("Race finished");
    }
    
    private void OnLapFinished()
    {
        //calculates how much faster (or slower, skill issue) that lap you just finished was compared to the last one
        lapTimeDifference = currentLapTime - prevLapTime;

        if (lapTimeDifference <= 0)
        {
            //green is good, - is to show the time was faster
            prevLapTimeDifferenceText.color = Color.green;
            prevLapTimeDifferenceText.text = "\t        -" + FormatTime(-lapTimeDifference);
        }
        else
        {
            //red bad, + mean more time bad not good very
            prevLapTimeDifferenceText.color = Color.red;
            prevLapTimeDifferenceText.text = "\t        +" + FormatTime(lapTimeDifference);
        }
        
        //sets how long the lap time difference text stays on screen for before fading
        //increments the lap
        //updates the previous lap time
        prevLapTimeDiffTextBuffer = 3f;
        currentLap++;
        prevLapTime = currentLapTime;
        
        respawnBoostCapsules();
        
        //ends the race when the total number of laps is exceeded, or resets the current lap time when starting a new lap
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
        //if you pass the first checkpoint and the race hasn't started, do that
        //if the race has started and the last checkpoint you passed was the one before the finish line, then call the on lap finished function
        if (checkpointIndex == 0)
        {
            if (!raceStarted)
            {
                StartRace();
            }
            
            else if (lastCheckpointIndex == checkpoints.Length - 1)
            {
                //Debug.Log("Lap completed");
                OnLapFinished();
            }
        }


        //sets the last checkpoint you passed
        lastCheckpointIndex = checkpointIndex;
        //Debug.Log("The last checkpoint is now " + lastCheckpointIndex);
    }

    public void CheckpointPassed(int checkpointIndex)
    {
        //makes all the checkpoints except the starting one do nothing if the race hasn't started
        if ((!raceStarted && checkpointIndex != 0) || raceFinished)
        {
            return;
        }

        //if the checkpoint you just passed isn't the second last one or the starting one, but the one after the previous one, update the checkpoint
        //otherwise you somehow missed one or went backwards, idiot
        if ((checkpointIndex == lastCheckpointIndex + 1) || (checkpointIndex == 0 && lastCheckpointIndex == checkpoints.Length - 1))
        {
            //Debug.Log("Last checkpoint was " + lastCheckpointIndex + ", so this is correct");
            UpdateCheckpoint(checkpointIndex);
        }
        else
        {
            //Debug.Log("Last checkpoint was " + lastCheckpointIndex + ", so this is wrong");
        }
    }

    private void respawnBoostCapsules()
    {
        foreach (GameObject boostCapsule in boostCapsules)
        {
            boostCapsule.SetActive(true);
        }
    }

    private void UpdateTimers()
    {
        //makes the timers tick and makes the UI buffer that controls that lap time difference text from earlier tick down
        currentLapTime += Time.deltaTime;
        totalRaceTime += Time.deltaTime;
        prevLapTimeDiffTextBuffer = Mathf.Lerp(prevLapTimeDiffTextBuffer, 0f, Time.deltaTime);
        trickScoreIncreaseTextBuffer -= Time.deltaTime;
        
        comboBuffer -= Time.deltaTime;
        
    }
    
    private void UpdateUI()
    {
        //sets the UI text to the correct values
        currentLapTimeText.text = "Lap Time: \t" + FormatTime(currentLapTime);
        totalRaceTimeText.text = "Time: \t" + FormatTime(totalRaceTime);
        lapText.text = "Lap: " + currentLap + "/" + totalLaps;
        prevLapTimeDifferenceText.alpha = prevLapTimeDiffTextBuffer * 85f;

        scoreText.text = "Score: \t" + String.Format("{0:0000000}", score);
        comboChainText.text = "" + comboChain;
        comboText.text = "COMBO: \tx" + comboMultiplier;
        comboSlider.value = comboGaugeInt;
        comboBufferText.text = FormatBuffer(comboBuffer);

        //turns off the UI and enables the FINISH! text when the race ends
        if (raceFinished)
        {
            currentLapTimeText.enabled = false;
            totalRaceTimeText.enabled = false;
            lapText.enabled = false;
            finishText.enabled = true;
            prevLapTimeDifferenceText.enabled = false;
        }

        if (comboBuffer <= 0)
        {
            comboBufferText.enabled = false;
            comboBufferSliderObject.SetActive(false);
        }
        else
        {
            comboBufferText.enabled = true;
            comboBufferSliderObject.SetActive(true);
        }

        if (trickScoreIncreaseTextBuffer <= 0)
        {
            trickScoreIncreaseText.enabled = false;
        }
        else
        {
            trickScoreIncreaseText.enabled = true;
        }
    }

    public void addComboChain()
    {
        comboBuffer = 5f;
        comboChain += 1;
        comboGaugeInt += 1;
        if (comboChain % 10 == 0)
        {
            comboMultiplier++;
            comboGaugeInt = 0;
        }
        addScore();
    }

    private void UpdateCombo()
    {
        if (comboBuffer <= 0)
        {
            comboChain = 0;
            comboGaugeInt = 0;
            comboMultiplier = 1;
        }
    }

    private void addScore()
    {
        int airTimeMultiplier = Mathf.RoundToInt(playerController.instance.groundedBuffer) * 3;
        if (airTimeMultiplier < 0)
        {
            airTimeMultiplier = airTimeMultiplier * -1;
        }

        int trickScore = 1 + (airTimeMultiplier * comboMultiplier);
        trickScoreIncreaseText.text = "Air Trick: +" + trickScore;
        trickScoreIncreaseTextBuffer = 0.5f;
        score += trickScore;
    }
    
    private string FormatTime(float time)
    {
        //formats the time to minutes:seconds.milliseconds
        if (float.IsInfinity(time) || float.IsNaN(time)) return "--:--.--";
        
        int minutes = (int)time / 60;
        float seconds = time % 60;
        
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    private string FormatBuffer(float time)
    {
        if (float.IsInfinity(time) || float.IsNaN(time)) return "--.--";
        
        return string.Format("{0:00.00}", time);
    }
}

using System;
using System.ComponentModel;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    //FMOD Event emitters
    [SerializeField]
    private FMODUnity.StudioEventEmitter boardHoverEmitter;
    [SerializeField]
    private FMODUnity.StudioEventEmitter windRushEmitter;
    [SerializeField]
    private FMODUnity.StudioEventEmitter trickEmitter;
    [SerializeField]
    private FMODUnity.StudioEventEmitter jumpEmitter;
    [SerializeField]
    private FMODUnity.StudioEventEmitter boostEmitter;
    
    //Min and max speed values for scaling the speed parameter
    [SerializeField]
    [Range(0.1f, 100f)]
    private float minSpeed = 3f;
    [SerializeField]
    [Range(0.1f, 300f)]
    private float maxSpeed = 150f;
    
    //Misc references for data to control other params
    [SerializeField]
    private playerController playerController;
    
    [SerializeField]
    private GameManager gameManager;
    
    [SerializeField]
    private Rigidbody playerRB;

    // Update is called once per frame
    void Update()
    {
        speedParamUpdate();
        boostParamUpdate();
        comboParamUpdate();
    }

    //Updates the speed parameter by taking the players speed and scaling it down to a value between 0 and 1
    void speedParamUpdate()
    {
        float speedRatio = Mathf.InverseLerp(minSpeed,maxSpeed,playerRB.linearVelocity.magnitude);
        float normalizedSpeed = Mathf.Lerp(0, 1, speedRatio);
        boardHoverEmitter.SetParameter("Speed", normalizedSpeed);
        windRushEmitter.SetParameter("Speed", normalizedSpeed);
    }

    //gets the current combo gauge fill value and uses that to adjust the combo param for the air trick noise
    void comboParamUpdate()
    {
        trickEmitter.SetParameter("Combo",gameManager.getComboGaugeInt());
    }
    
    //converts the public isBoosting bool from playercontroller to and int and uses that as the boosting param
    void boostParamUpdate()
    {
        int isBoostingInt = Convert.ToInt32(playerController.isBoosting);
        boardHoverEmitter.SetParameter("Boosting", isBoostingInt);
        windRushEmitter.SetParameter("Boosting", isBoostingInt);
    }

    //Is called by playercontroller under the jump function
    public void startJumpEmitter()
    {
        jumpEmitter.Play();
    }
}

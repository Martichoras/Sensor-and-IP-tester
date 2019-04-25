using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UDPBikeAcceleration : MonoBehaviour
{

    public BangInterpreter bangInterpreter;
    public UDP udp;
    //public float speedConstant = 0.1f;
    public float decelerationConstant = 0.2f;
    public float accelerationConstant = 1;

    float currentVelAim = 0;
    public float currentVel = 0;

    public bool TestMode = false;

    float velocityDifference = 0;
    List<Vector3> velocityRecord = new List<Vector3>();
    List<Vector3> revRecord = new List<Vector3>();

    //private GameObject CameraController;
    //public Ardunio2Unity A2U;


    void FixedUpdate()
    {
        if (bangInterpreter.CheckRevPerSecReady(1))
        {
            // Get the time since the last bang, but cap it so that it never is smaller than the previous interval on record.
            if (Time.time - bangInterpreter.GetTimeStamp(0) < bangInterpreter.GetTimeStamp(0) - bangInterpreter.GetTimeStamp(1))
            {
                // Carry on as usual...
                currentVelAim = bangInterpreter.GetRevPerSec(0);
            }
            else
            {
                // We are overshooting the expected time

                currentVelAim = bangInterpreter.RevolutionsPerSecond;
            }

        }
        else if (bangInterpreter.CheckRevPerSecReady(0))
        {
            currentVelAim = bangInterpreter.GetRevPerSec(0);
        }

        if (TestMode)
            currentVelAim = 600.0f;
        else
            currentVelAim = udp.speed;
        //currentVelAim = udp.speed; // SWITCH TO THIS IF UDP IS NEEDED!

        currentVel = currentVel + Mathf.Max((currentVelAim - currentVel) * accelerationConstant, -decelerationConstant) * Time.fixedDeltaTime;

    }
}

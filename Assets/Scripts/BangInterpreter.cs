using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This class tries to detect input bangs every FixedUpdate, and records their timing.
 * It then gives an estimate of revolutions per second, which can be read from other scripts.
 * 
 * 
 */

public class BangInterpreter : MonoBehaviour {

    bool bang = false;
    public int listLength = 20;

    public float maximumTimeBetweenBangs = 3f;
    public float minimumRevsPerSec = 0.5f;
    [SerializeField]
    float revolutionsPerSecond = 0; // The current revolutions per second, measured using the last time difference between bangs

    List<float> timeStamps = new List<float>();
    List<float> revPerSecHistory = new List<float>();

    public float RevolutionsPerSecond { get { return revolutionsPerSecond; } }
    public List<float> TimeStamps { get { return timeStamps; } }
    public List<float> RevPerSecHistory { get { return revPerSecHistory; } }

    /// <summary>
    /// Gets an entry from the 'revPerSec' list. Index 0 returns the very last entry, index 1 returns the second-to-last, and so forth.
    /// </summary>
    /// <param name="indexFromEnd"></param>
    /// <returns></returns>
    public float GetTimeStamp(int indexFromEnd = 0) {
        if (indexFromEnd >= 0 && indexFromEnd <= timeStamps.Count - 1) {
            return timeStamps[timeStamps.Count - 1 - indexFromEnd];
        } else {
            Debug.LogError("Trying to access index out of bounds - index must be between 0 and the current count of the list 'timeStamps'. Tried to get index " + indexFromEnd + ", list count was " + timeStamps.Count);
            return 0;
        }
    }

    public bool CheckTimeStampReady(int index) {
        if (timeStamps.Count > index) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Gets an entry from the 'revPerSec' list. Index 0 returns the very last entry, index 1 returns the second-to-last, and so forth.
    /// </summary>
    /// <param name="indexFromEnd"></param>
    /// <returns></returns>
    public float GetRevPerSec(int indexFromEnd = 0) {
        if (indexFromEnd >= 0 && indexFromEnd <= revPerSecHistory.Count - 1) {
            return revPerSecHistory[revPerSecHistory.Count - 1 - indexFromEnd];
        } else {
            Debug.LogError("Trying to access index out of bounds - index must be between 0 and the current count of the list 'revPerSecHistory'. Tried to get index " + indexFromEnd + ", list count was " + revPerSecHistory.Count);
            return 0;
        }
    }

    public bool CheckRevPerSecReady(int index) {
        if (revPerSecHistory.Count > index) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Returns the revolutions per second, given a time difference. Will never be lower than the 'minimumRevsPerSec'.
    /// </summary>
    /// <param name="timeDifference"></param>
    /// <returns></returns>
    private float CalcRevPerSec(float timeDifference) {
        return Mathf.Max(1f / timeDifference, minimumRevsPerSec);
    }

    /// <summary>
    /// Call this method to tell the system that a bang has occured on the Arduino
    /// </summary>
    public void BANG() {
        bang = true;
    }


    //----------------------------------------------------

    void Start() {
        // Populate arrays with reasonable input to simulate standing still
        for (int i = -10; i <= -1; i++) {
            timeStamps.Add(i * (maximumTimeBetweenBangs + 0.001f));
            revPerSecHistory.Add(0);
        }
    }

    void FixedUpdate() {

        float lastTimeDifference = timeStamps[timeStamps.Count - 1] - timeStamps[timeStamps.Count - 2];
        float currentTimeDifference = Time.time - timeStamps[timeStamps.Count - 1];


        // If input has been received...
        if (bang) {
            timeStamps.Add(Time.time); // Record the time at which the input was received...

            // Use this to generate a current Revs Per Second value
            revolutionsPerSecond = CalcRevPerSec(currentTimeDifference);
            revPerSecHistory.Add(revolutionsPerSecond);


        } else {
            // No input and we're outside the reasonable limit for continuing bike movement; kill the revolutions per second.
            if (currentTimeDifference > maximumTimeBetweenBangs) {
                if (revolutionsPerSecond != 0) {
                    revolutionsPerSecond = 0;
                    revPerSecHistory.Add(revolutionsPerSecond);
                }

            } else {
                // No input... running check of revs per second
                revolutionsPerSecond = CalcRevPerSec(Mathf.Max(currentTimeDifference, lastTimeDifference));

            }
        }

        // Make sure the list does not become longer than 'listLength'...
        if (timeStamps.Count > listLength) {
            timeStamps.RemoveAt(0);
        }

        // Make sure the list does not become longer than 'listLength'...
        if (revPerSecHistory.Count > listLength) {
            revPerSecHistory.RemoveAt(0);
        }

        // Reset the bang
        bang = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public TMP_Text timerText;
    public float timer = 60.0f;
    public bool counting = true;
    public AppleGame mainScript;

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            timer -= Time.deltaTime;
            string TimerString = timer.ToString("0");
            timerText.SetText("Time: " + TimerString);
            if (timer <= 0)
            {
                EndGame();
            }
        }
                
    }

    public void EndGame()
    {
        counting = false;
        timer = 60.0f;
        mainScript.GameOver();
    }
}

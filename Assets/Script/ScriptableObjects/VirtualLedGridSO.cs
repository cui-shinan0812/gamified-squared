using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VirtualLedGridSO {
    private float redToYellowCountDownTimeMax;
    private float redToYellowCountDownTime;

    public VirtualLedGridSO()
    {
        redToYellowCountDownTimeMax = 1;
        InitObject();
    }
    
    public void InitObject()
    {
        redToYellowCountDownTime = redToYellowCountDownTimeMax;
    }

    public void SetRedToYellowCountDownTime(float timeToDeduct)
    {
        if (redToYellowCountDownTime > 0)
        {
            redToYellowCountDownTime -= timeToDeduct;
        }
    }

    public float GetRedToYellowCountDownTime()
    {
        return redToYellowCountDownTime;
    }

    public int GetRedToYellowCountDownTimeCeilToInt()
    {
        return Mathf.CeilToInt(redToYellowCountDownTime);
    }

    public float GetRedToYellowCountDownTimeMax()
    {
        return redToYellowCountDownTimeMax;
    }
}
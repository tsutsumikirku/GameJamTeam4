using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public float gameTime = 180f;
    float currentTime;
    bool isRunnning = false;
    public event Action OnTimerEnd;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!isRunnning) return;
        currentTime -= Time.deltaTime;

        if(currentTime <= 0)
        {
            currentTime = 0;
            isRunnning = false;
            OnTimerEnd?.Invoke();
        }
    }

    public void StartTimer()
    {
        currentTime = gameTime;
        isRunnning=true;
    }

    public float GetTime()
    {
        return currentTime;
    }

}

using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [SerializeField] TextMeshProUGUI timerText;
    public float gameTime = 180f;
    float currentTime;
    bool isRunnning = false;
    public Action OnTimerEnd;

    private void Awake()
    {
        instance = this;
        timerText.gameObject.SetActive(false);
        timerText.text = gameTime.ToString();
    }

    private void Update()
    {
        if (!isRunnning) return;
        currentTime -= Time.deltaTime;
            timerText.text = Mathf.Ceil(currentTime).ToString();
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
        isRunnning = true;
        timerText.gameObject.SetActive(true);
    }

    public float GetTime()
    {
        return currentTime;
    }

}

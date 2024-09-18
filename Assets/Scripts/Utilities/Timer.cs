using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public bool IsRunningCoroutine = false;

    private float _timerDuration;
    private float _timer;

    public float TimerDuration
    {
        get { return _timerDuration; }
        set { _timerDuration = value; }
    }

    public float TimeRemaining {  get { return _timer; } }  

    public Timer( float timerDuration)
    {
        _timerDuration = timerDuration;
    }




    //Coroutine based timer
    public void StartTimerCoroutine()
    {
        if (!IsRunningCoroutine)
        {
            CoroutineManager.Instance.Coroutine(TimerCoroutine());
        }
    }

    private IEnumerator TimerCoroutine()
    {
        IsRunningCoroutine = true;
        yield return new WaitForSeconds(_timerDuration);
        IsRunningCoroutine = false;
    }

    public void StopTimerCoroutine()
    {
        if (IsRunningCoroutine)
        {
            CoroutineManager.Instance.StopCoroutine(TimerCoroutine());
            IsRunningCoroutine = false;
        }
    }






    //Basic timer
    public void UpdateTimerBasic(float deltaTime)
    {
        if (_timer > 0)
        {
            _timer -= deltaTime;
        }
        else
        {
            _timer = 0;
        }
    }

    public void StartTimerBasic()
    {
        _timer = _timerDuration;
    }


    public bool IsRunningBasic()
    {
        return _timer > 0;
    }

    public void StopTimerBasic()
    {
        _timer = 0;
    }
}

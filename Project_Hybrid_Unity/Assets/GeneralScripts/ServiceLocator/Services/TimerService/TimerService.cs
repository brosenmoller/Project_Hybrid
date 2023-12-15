using System;
using UnityEngine;

public class TimerService : Service
{
    public event Action<float> OnTimerUpdate;
    public override void OnUpdate()
    {
        OnTimerUpdate?.Invoke(Time.fixedDeltaTime);
    }
}

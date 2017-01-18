using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct EventTicker
{
    int tickCountdown_;
    /// <summary>
    /// Counter that decrements once per tick. When this reaches 0, the action will be invoked.
    /// </summary>
    //public int TickCountdown_
    //{
    //    get { return tickCountdown_; }
    //    set
    //    {
    //        tickCountdown_ = value;
    //        if (tickCountdown_ <= 0)
    //            OnComplete_.Invoke();
    //    }
    //}

    

    /// <summary>
    /// Action called when ticks reaches 0.
    /// </summary>
    System.Action onComplete_;

    public void Tick()
    {
        --tickCountdown_;
        if (tickCountdown_ <= 0)
            onComplete_.Invoke();
    }

    public EventTicker(int ticks, System.Action onComplete)
    {
        tickCountdown_ = ticks;
        onComplete_ = onComplete;
    }
}

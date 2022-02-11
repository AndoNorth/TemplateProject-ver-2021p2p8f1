using System;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour
{
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    public static event EventHandler<OnTickEventArgs> OnTick;
    public static event EventHandler<OnTickEventArgs> OnTick_Mod5;

    private float _tickRate = .2f; // 20ms
    private int _tick;
    private float _tickTimer;
    public void TimeTicketSystem(float tickRate)
    {
        this._tickRate = tickRate;
    }
    private void Awake()
    {
        _tick = 0;
    }
    void Update()
    {
        _tickTimer += Time.deltaTime;
        if(_tickTimer >= _tickRate)
        {
            _tickTimer -= _tickRate;
            _tick++;
            if (OnTick != null)
            {
                OnTick(this, new OnTickEventArgs { tick = _tick }); // TimeTicketSystem.OnTick += delegate (object sender, TimeTickSystem.OnTickEventArgs event) { };
            }
            if (_tick % 5 == 0 && OnTick_Mod5 != null)
            {
                OnTick_Mod5(this, new OnTickEventArgs { tick = _tick });
            }
        }
    }
}

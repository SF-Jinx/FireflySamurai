using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Timers
{
    public float DefaultTime { get; set; }
    public float CurrentTime { get; set; }
    public float DefaultCooldownTime { get; set; }
    public float CurrentCooldownTime { get; set; }

    public Timers(  float defaultTime, float currentTime, float defaultCooldownTime, float currentCooldownTime)
    {
        DefaultTime = defaultTime;
        CurrentTime = currentTime;
        DefaultCooldownTime = defaultCooldownTime;
        CurrentCooldownTime = currentCooldownTime;
    }

    public void updateValues(float defaultTime, float currentTime, float defaultCooldownTime, float currentCooldownTime)
    {
        DefaultTime = defaultTime;
        CurrentTime = currentTime;
        DefaultCooldownTime = defaultCooldownTime;
        CurrentCooldownTime = currentCooldownTime;
    }

    public void updateCooldown(float newTime)
    {
        CurrentCooldownTime = newTime;
    }

    public void updateCurrentTime(float newTime)
    {
        CurrentTime = newTime;
    }
}
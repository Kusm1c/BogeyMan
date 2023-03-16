using System.Collections.Generic;
using UnityEngine;

public class Clock
{
    private static readonly List<Clock> allClocks = new();
    
    public bool isRunning { get; private set; }
    public bool isPaused { get; private set; }

    private float startTime;
    private float pauseTime;
    private float pausedDuration;

    public Clock()
    {
        allClocks.Add(this);
    }

    ~Clock()
    {
        allClocks.Remove(this);
    }

    public void Start(float overrideStartTime = 0f)
    {
        startTime = Time.time - overrideStartTime;
        pausedDuration = 0f;
        isPaused = false;
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Pause()
    {
        pauseTime = Time.time;
        isPaused = true;
    }

    public void Resume()
    {
        pausedDuration += Time.time - pauseTime;
        isPaused = false;
    }

    public void Restart() => Start();

    public float GetTime()
    {
        if (!isRunning)
        {
            return 0;
        }

        return Time.time - startTime - pausedDuration;
    }

    public static void PauseAll()
    {
        foreach (Clock clock in allClocks)
        {
            clock.Pause();
        }
    }
    
    public static void ResumeAll()
    {
        foreach (Clock clock in allClocks)
        {
            clock.Resume();
        }
    }
}
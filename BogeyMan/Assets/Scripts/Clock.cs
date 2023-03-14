using UnityEngine;

public class Clock
{
    public bool isRunning { get; private set; }
    public bool isPaused { get; private set; }

    private float startTime;
    private float pauseTime;
    private float pausedDuration;

    public void Start()
    {
        startTime = Time.time;
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
}
namespace Oxard.Common.Diagnostics;

public class Timer
{
    private readonly TimeSpan interval;
    private readonly Action callbackOnTick;
    private readonly object locker = new object();
    private CancellationTokenSource? cancellationTokenSource;
    private Task? timerLoopTask;
    private bool isStarted;

    public Timer(TimeSpan interval, Action callbackOnTick)
    {
        this.interval = interval;
        this.callbackOnTick = callbackOnTick;
    }

    public void Start()
    {
        lock (locker)
        {
            if (isStarted)
                return;

            isStarted = true;
        }

        cancellationTokenSource = new CancellationTokenSource();
        timerLoopTask = TimerLoop();
    }

    public void Stop()
    {
        lock (locker)
        {
            if (!isStarted)
                return;

            isStarted = false;
        }

        cancellationTokenSource?.Cancel();
        try
        {
            timerLoopTask?.Wait();
        }
        catch (AggregateException e)
        {
            if (e.InnerExceptions.Count != 1 || !(e.InnerExceptions[0] is TaskCanceledException))
            {
                // There is a bug in timer loop callback
                throw;
            }

            // It's OK, task is finished
        }
    }

    private async Task TimerLoop()
    {
        if (cancellationTokenSource == null)
            await Task.Delay((int)interval.TotalMilliseconds);
        else
        {
            await Task.Delay((int)interval.TotalMilliseconds, cancellationTokenSource.Token);

            if (cancellationTokenSource.IsCancellationRequested)
                return;
        }

        callbackOnTick();
        await TimerLoop();
    }
}
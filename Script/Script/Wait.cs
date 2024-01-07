using System;
using System.Threading.Tasks;

public static class Wait
{
    public static async Task WaitTime(float time, Action action)
    {
        await Task.Delay(TimeSpan.FromSeconds(time));
        action();
    }

    public static float WaitTimer = 1.5f;
}


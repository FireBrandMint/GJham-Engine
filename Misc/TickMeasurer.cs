using System;

public class TickMeasurer
{
    Int64 lastTick = MainClass.TickCount;

    int TicksExecuted = 0;

    public void Update()
    {
        TicksExecuted = (int)(MainClass.TickCount - lastTick);
        
        lastTick = MainClass.TickCount;
    }

    public int GetTicksThisSecond ()
    {
        return TicksExecuted;
    }
}
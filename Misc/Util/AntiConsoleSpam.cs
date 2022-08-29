using System;
using System.Collections.Generic;

public class AntiConsoleSpam
{
    public static AntiConsoleSpam antiConsoleSpam = new AntiConsoleSpam();

    Dictionary<int, int> MessageAsktimeDict = new Dictionary<int, int>();

    public void WriteLine (ref string message, int ID, int ticksToWait)
    {
        if (MessageAsktimeDict.ContainsKey(ID))
        {
            MessageAsktimeDict[ID]+=1;
            int currTick = MessageAsktimeDict[ID];

            if (currTick >= ticksToWait)
            {
                Console.WriteLine(message);

                MessageAsktimeDict[ID] = 0;
            }
            else
            {
                MessageAsktimeDict[ID] = currTick;
            }
        }
        else
        {
            MessageAsktimeDict.Add(ID, 1);
        }
    }

    public bool CanWriteLine (int ID, int ticksToWait)
    {
        if (MessageAsktimeDict.ContainsKey(ID))
        {
            MessageAsktimeDict[ID]+=1;
            int currTick = MessageAsktimeDict[ID];

            if (currTick >= ticksToWait)
            {

                MessageAsktimeDict[ID] = 0;

                return true;
            }
            else
            {
                MessageAsktimeDict[ID] = currTick;

                return false;
            }
        }

        MessageAsktimeDict.Add(ID, 1);

        return false;
    }

    public void RemoveIDUnsafe (int ID)
    {
        MessageAsktimeDict.Remove(ID);
    }

    public void RemoveIDSafe (int ID)
    {
        if (MessageAsktimeDict.ContainsKey(ID)) MessageAsktimeDict.Remove(ID);
    }
}
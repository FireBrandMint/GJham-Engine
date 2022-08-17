using System.Diagnostics;

public static class STime
{
    public static long GetMicroseconds()
	{
		long timestamp = Stopwatch.GetTimestamp();
		long microseconds = 1_000_000 * timestamp / Stopwatch.Frequency;

		return (long)microseconds;
	}
}
namespace AdventOfCode.Models;

public record Interval(int Start, int End)
{
    public bool IsZero()
    {
        return Start == 0 && End == 0;
    }

    public int CountRange()
    {
        return End - Start;
    }
}
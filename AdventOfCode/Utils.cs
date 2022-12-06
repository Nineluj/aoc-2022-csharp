namespace AdventOfCode;

public static class Utils
{
    public static int Mod(int x, int m) {
        return (x%m + m)%m;
    }

    public static IEnumerable<string> GetLines(string input)
    {
        return input.Split(Environment.NewLine).Where(line => line.Length != 0);
    }
}
namespace AdventOfCode;

public sealed class Day01 : CustomDirBaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var packing = GetPackList(_input);
        var calories = packing.Select(Enumerable.Sum).ToList();
        calories.Sort();
        var result = calories.Last();
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var packing = GetPackList(_input);
        var calories = packing.Select(Enumerable.Sum).ToList();
        calories.Sort();
        return new ValueTask<string>(calories.TakeLast(3).Sum().ToString());
    }

    private List<List<int>> GetPackList(string input)
    {
        var collect = new List<List<int>>();
        var curr = new List<int>();
        foreach (var line in input.Split(Environment.NewLine))
        {
            if (line.Length == 0)
            {
                collect.Add(curr);
                curr = new List<int>();
            }
            else
            {
                curr.Add(int.Parse(line));
            }
        }

        return collect;
    }
}

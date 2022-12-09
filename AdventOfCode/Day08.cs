namespace AdventOfCode;

public sealed class Day08 : CustomDirBaseDay
{
    private readonly string _input;

    public Day08()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private List<List<int>> GetGrid(string input)
    {
        return Utils.GetLines(input)
            .Select(line => line
                .Select(c => c - '0').ToList())
            .ToList();
    }

    private int CountBetween(
        IReadOnlyList<List<int>> grid,
        int xStart, int xEnd, int xStep, Func<int, int, bool> xComp,
        int yStart, int yEnd, int yStep, Func<int, int, bool> yComp,
        ISet<string> seenPositions)
    {
        var currMax = int.MinValue;
        var count = 0;
        for (var y = yStart; yComp(y, yEnd); y += yStep)
        for (var x = xStart; xComp(x, xEnd); x += xStep)
        {
            var positionString = $"{x}-{y}";
            // Console.WriteLine(positionString);
            if (grid[y][x] > currMax)
            {
                currMax = grid[y][x];
                if (!seenPositions.Contains(positionString))
                {
                    count++;
                    seenPositions.Add(positionString);
                }
            }
        }

        return count;
    }

    private int CountVisibleGrid(List<List<int>> grid)
    {
        var countTotal = 0;
        var seenPositions = new HashSet<string>();

        var yMax = grid.Count;
        var xMax = grid[0].Count;

        bool Lt(int a, int b)
        {
            return a < b;
        }

        bool Gte(int a, int b)
        {
            return a >= b;
        }

        bool Eq(int a, int b)
        {
            return a == b;
        }

        // count everything going horizontally
        for (var y = 0; y < yMax; y++)
        {
            countTotal += CountBetween(grid, 0, xMax, 1, Lt, y, y, 1, Eq, seenPositions);
            countTotal += CountBetween(grid, xMax - 1, 0, -1, Gte, y, y, 1, Eq, seenPositions);
        }

        // count everything going vertically
        for (var x = 0; x < xMax; x++)
        {
            countTotal += CountBetween(grid, x, x, 1, Eq, 0, yMax, 1, Lt, seenPositions);
            countTotal += CountBetween(grid, x, x, 1, Eq, yMax - 1, 0, -1, Gte, seenPositions);
        }

        return countTotal;
    }

    public override ValueTask<string> Solve_1()
    {
        var grid = GetGrid(_input);

        var result = CountVisibleGrid(grid);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }
}
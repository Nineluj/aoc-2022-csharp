using System.Diagnostics;
using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day09 : CustomDirBaseDay
{
    private readonly string _input;

    public Day09()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<Movement> GetMovements(string input)
    {
        return Utils.GetLines(input)
            .Select(line => new Movement((Direction)line[0], int.Parse(line[2..]))
            );
    }

    private void DrawGrid(Vector2Int head, Vector2Int tail, int xMax, int yMax)
    {
        Console.WriteLine("=======");
        Console.WriteLine($"head={head}, tail={tail}");
        for (var y = yMax - 1; y >= 0; y--)
        {
            for (var x = 0; x < xMax; x++)
            {
                var c = new Vector2Int(x, y);
                Console.Write(c.Equals(head) ? 'H' : c.Equals(tail) ? 'T' : '.');
            }

            Console.WriteLine();
        }

        Console.WriteLine("=======");
    }

    private ISet<Vector2Int> SimulateMovements(IEnumerable<Movement> movements, int ropeLength)
    {
        var ropeCoordinates = Enumerable
            .Range(0, ropeLength)
            .Select(_ => new Vector2Int(0, 0))
            .ToList();

        var seen = new HashSet<Vector2Int>();
        foreach (var move in movements)
            for (var i = 0; i < move.Magnitude; i++)
            {
                if (Debugger.IsAttached) DrawGrid(ropeCoordinates[0], ropeCoordinates[^1], 25, 25);
                seen.Add(ropeCoordinates[^1]);

                ropeCoordinates[0] = ropeCoordinates[0].ToDirection(move.D);
                for (var ropeIndex = 1; ropeIndex < ropeLength; ropeIndex++)
                    ropeCoordinates[ropeIndex] = PullTowards(
                        ropeCoordinates[ropeIndex],
                        ropeCoordinates[ropeIndex - 1]);
            }

        seen.Add(ropeCoordinates[^1]);
        return seen;
    }

    public override ValueTask<string> Solve_1()
    {
        var result = SimulateMovements(GetMovements(_input), 2).Count;
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = SimulateMovements(GetMovements(_input), 10).Count;

        return new ValueTask<string>(result.ToString());
    }


    private Vector2Int PullTowards(Vector2Int origin, Vector2Int other)
    {
        var delta = other.Diff(origin);
        if (WithinNoPullRange(delta)) return origin;

        if (delta.X == 0)
            return delta.Y switch
            {
                > 0 => origin with { Y = origin.Y + 1 },
                < 0 => origin with { Y = origin.Y - 1 },
                _ => throw new ArgumentOutOfRangeException()
            };
        if (delta.Y == 0)
            return delta.X switch
            {
                > 0 => origin with { X = origin.X + 1 },
                < 0 => origin with { X = origin.X - 1 },
                _ => throw new ArgumentOutOfRangeException()
            };

        var unitVec = delta.ToUnitVector();
        return new Vector2Int(origin.X + unitVec.X, origin.Y + unitVec.Y);
    }


    private static bool WithinNoPullRange(Vector2Int delta)
    {
        return Math.Abs(delta.X) <= 1 && Math.Abs(delta.Y) <= 1;
    }

    private record Movement(Direction D, int Magnitude);
}
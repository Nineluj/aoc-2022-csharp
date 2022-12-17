using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day17 : CustomDirBaseDay
{
    private static readonly List<Vector2Int> HorizontalLineShape = new()
    {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0)
    };

    private static readonly List<Vector2Int> PlusShape = new()
    {
        new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(2, 1), new Vector2Int(1, 2)
    };

    private static readonly List<Vector2Int> ReverseLShape = new()
    {
        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2)
    };

    private static readonly List<Vector2Int> VerticalLineShape = new()
    {
        new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3)
    };

    private static readonly List<Vector2Int> BoxShape = new()
    {
        new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1)
    };

    private static readonly List<Vector2Int>[] ShapeForIndex =
    {
        HorizontalLineShape, PlusShape, ReverseLShape, VerticalLineShape, BoxShape
    };

    private readonly string _input;

    public Day17()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public IEnumerable<Direction> ParseInput(string input)
    {
        return input.Select(c => c == '>'
            ? Direction.Right
            : c == '<'
                ? Direction.Left
                : throw new ArgumentException($"Not a valid direction: {c}"));
    }

    private MovingShape GetNextShape(int i, List<bool[]> state)
    {
        var yOffset = state.Select((line, index) => line.Any(c => c) ? index : 0).Max() + 3;
        return new MovingShape(ShapeForIndex[i % ShapeForIndex.Length], new Vector2Int(2, yOffset));
    }

    private int Simulate(List<Direction> moves, int maxRocks)
    {
        var stoppedRocks = 0;
        var heightOffset = 0;
        var state = Enumerable
            .Range(0, 4)
            .Select(_ => Enumerable
                .Range(0, 7)
                .Select(_ => false)
                .ToArray())
            .ToList();

        MovingShape shape;
        var moveIndex = 0;
        while (stoppedRocks < maxRocks)
        {
            shape = GetNextShape(moveIndex, state);

            moveIndex++;


            // checked the row where each cell of the shape stopped. If full,
            // remove ALL the rows below it, increment heightOffset
        }

        return state.Count + heightOffset;
    }

    public override ValueTask<string> Solve_1()
    {
        var moves = ParseInput(_input);
        var result = Simulate(moves.ToList(), 2022);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private record MovingShape(List<Vector2Int> Points, Vector2Int Offset)
    {
        // public void Move()
    }
}

// private static CellState[,] HorizontalLineShape = new CellState[1, 4]
//     { { CellState.FallingBlock, CellState.FallingBlock, CellState.FallingBlock, CellState.FallingBlock } };
//
// private static CellState[,] PlusShape = new CellState[3, 3]
// {
//     { CellState.Empty, CellState.FallingBlock, CellState.Empty },
//     { CellState.FallingBlock, CellState.FallingBlock, CellState.FallingBlock },
//     { CellState.Empty, CellState.FallingBlock, CellState.Empty }
// };
//
//
// private static CellState[,] ReverseLShape = new CellState[3, 3]
// {
//     { CellState.Empty, CellState.Empty, CellState.FallingBlock },
//     { CellState.Empty, CellState.Empty, CellState.FallingBlock },
//     { CellState.FallingBlock, CellState.FallingBlock, CellState.FallingBlock }
// };
//
// private static CellState[,] VerticalLineShape = new CellState[4, 1]
// {
//     { CellState.FallingBlock },
//     { CellState.FallingBlock },
//     { CellState.FallingBlock },
//     { CellState.FallingBlock }
// };
//
// private static CellState[,] BoxShape = new CellState[2, 2]
// {
//     { CellState.FallingBlock, CellState.FallingBlock },
//     { CellState.FallingBlock, CellState.FallingBlock }
// };
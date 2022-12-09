namespace AdventOfCode.Models;

public record Vector2Int(int X, int Y)
{
    public Vector2Int ToDirection(Direction d)
    {
        return d switch
        {
            Direction.Up => this with { Y = Y + 1 },
            Direction.Down => this with { Y = Y - 1 },
            Direction.Left => this with { X = X - 1 },
            Direction.Right => this with { X = X + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
        };
    }

    public Vector2Int Diff(Vector2Int other)
    {
        return new Vector2Int(X - other.X, Y - other.Y);
    }

    /// <summary>
    ///     Get the Manhattan distance version of this vector, ie the move which is at
    ///     most 1 North/South and 1 East/West in the direction of the original vector.
    /// </summary>
    public Vector2Int ToManhattanUnitVector()
    {
        return new Vector2Int(X == 0 ? 0 : X / Math.Abs(X), Y == 0 ? 0 : Y / Math.Abs(Y));
    }
}
namespace AdventOfCode.Models;

public record Vector2Int(int X, int Y)
{
    public static Vector2Int DownPosY = new(0, 1);
    public static Vector2Int Right = new(1, 0);


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

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new(a.X + b.X, a.Y + b.Y);
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
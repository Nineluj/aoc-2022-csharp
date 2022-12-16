namespace AdventOfCode.Models;

public record Vector2Int(int X, int Y)
{
    public static Vector2Int PosY = new(0, 1);
    public static Vector2Int PosX = new(1, 0);


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
        return new Vector2Int(a.X + b.X, a.Y + b.Y);
    }

    // public static bool operator <(Vector2Int a, Vector2Int b)
    //     => ;

    public int ManhattanDistanceTo(Vector2Int other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public bool WithinRange(Vector2Int range, Vector2Int otherPoint)
    {
        return ManhattanDistanceTo(otherPoint) <= ManhattanDistanceTo(range);
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
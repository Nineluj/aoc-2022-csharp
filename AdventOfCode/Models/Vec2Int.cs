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

    public Vector2Int ToUnitVector()
    {
        return new Vector2Int(X / Math.Abs(X), Y / Math.Abs(Y));
    }
}
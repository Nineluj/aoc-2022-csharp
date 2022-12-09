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

    private void DrawGrid(Coordinate2D head, Coordinate2D tail)
    {
        Console.WriteLine("=======");
        for (var y = 4; y >= 0; y--)
        {
            for (var x = 0; x < 5; x++)
            {
                var c = new Coordinate2D(x, y);
                Console.Write(c.Equals(head) ? 'H' : c.Equals(tail) ? 'T' : '.');
            }

            Console.WriteLine();
        }

        Console.WriteLine("=======");
    }

    private ISet<Coordinate2D> SimulateMovements(IEnumerable<Movement> movements)
    {
        var h = new Coordinate2D(0, 0);
        var t = new Coordinate2D(0, 0);
        var seen = new HashSet<Coordinate2D>();
        foreach (var move in movements)
            for (var i = 0; i < move.Magnitude; i++)
            {
                // Console.WriteLine($"head={h}, tail={t}");
                // DrawGrid(h, t);
                seen.Add(t);
                h = h.ToDirection(move.D);
                t = t.PullTowards(h);
            }

        return seen;
    }

    public override ValueTask<string> Solve_1()
    {
        var result = SimulateMovements(GetMovements(_input)).Count();
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private enum Direction
    {
        Up = 'U',
        Down = 'D',
        Left = 'L',
        Right = 'R'
    }

    private record Movement(Direction D, int Magnitude);

    private record Coordinate2D(int X, int Y)
    {
        public Coordinate2D ToDirection(Direction d)
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

        public Coordinate2D Diff(Coordinate2D other)
        {
            return new Coordinate2D(X - other.X, Y - other.Y);
        }

        public bool WithinNoPullRange(Coordinate2D delta)
        {
            return Math.Abs(delta.X) <= 1 && Math.Abs(delta.Y) <= 1;
        }

        public Direction? SameDirectionAs(Coordinate2D other)
        {
            if (X == other.X)
            {
                if (Y > other.Y) return Direction.Down;
                if (Y < other.Y) return Direction.Up;
            }

            if (Y == other.Y)
            {
                if (X > other.X) return Direction.Left;
                if (X < other.X) return Direction.Right;
            }

            return null;
        }

        public Coordinate2D ToUnitVector()
        {
            return new Coordinate2D(X / Math.Abs(X), Y / Math.Abs(Y));
        }

        public Coordinate2D PullTowards(Coordinate2D other)
        {
            var delta = other.Diff(this);
            if (WithinNoPullRange(delta)) return this;

            if (delta.X == 0)
                return delta.Y switch
                {
                    > 0 => this with { Y = Y + 1 },
                    < 0 => this with { Y = Y - 1 },
                    _ => throw new ArgumentOutOfRangeException()
                };
            if (delta.Y == 0)
                return delta.X switch
                {
                    > 0 => this with { X = X + 1 },
                    < 0 => this with { X = X - 1 },
                    _ => throw new ArgumentOutOfRangeException()
                };

            var unitVec = delta.ToUnitVector();
            return new Coordinate2D(X + unitVec.X, Y + unitVec.Y);
        }
    }
}
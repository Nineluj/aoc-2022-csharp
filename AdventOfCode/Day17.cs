using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day17 : CustomDirBaseDay
{
    private readonly string _input;

    public Day17()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private static IEnumerable<Direction> ParseInput(string input)
    {
        return Utils.GetLines(input).ToList()[0].Select(c => c switch
        {
            '>' => Direction.Right,
            '<' => Direction.Left,
            _ => throw new ArgumentException($"Not a valid direction: {c}")
        });
    }

    private async Task<int> Simulate(List<Direction> moves, int maxRocks)
    {
        /*
        var stoppedRocks = 0;
        var heightOffset = 0;

        Shape movingShape;
        var needCreateShape = true;
        var moveIndex = 0;
        */

        var moveIndex = 0;
        var game = new TetrisGame();
        // game.Draw();
        while (game.ShapeIndex < 2022) //stoppedRocks < maxRocks)
        {
            game.Tick(moves[moveIndex % moves.Count]);
            // game.Draw();
            moveIndex++;
            // await Task.Delay(500);
            // checked the row where each cell of the shape stopped. If full,
            // remove ALL the rows below it, increment heightOffset
        }

        return game.GetHeight(); // board.Count + heightOffset;
    }

    public override async ValueTask<string> Solve_1()
    {
        var moves = ParseInput(_input);
        var result = await Simulate(moves.ToList(), 2022);
        return result.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        // throw new NotImplementedException();
        return new ValueTask<string>(result);
    }

    private class Shape
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

        public List<Vector2Int> Points;

        public Shape(int i, int currentMaxHeight)
        {
            var vectorOffset = new Vector2Int(2, currentMaxHeight + 3);
            Points = ShapeForIndex[i % ShapeForIndex.Length].Select(x => x + vectorOffset).ToList();
        }

        public bool Move(Direction d, Func<Vector2Int, bool> hasCollision)
        {
            var vec = d switch
            {
                Direction.Left => new Vector2Int(-1, 0),
                Direction.Right => new Vector2Int(1, 0),
                Direction.Down => new Vector2Int(0, -1),
                Direction.Up => throw new ArgumentException("Cannot move shape up."),
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
            var candidatePoints = Points.Select(x => x + vec).ToList();
            if (candidatePoints.Any(hasCollision)) return true;
            Points = candidatePoints;
            return false;
        }
    }

    public class TetrisGame
    {
        private readonly List<bool[]> _board;
        private Shape _curr;
        public int ShapeIndex;

        public TetrisGame()
        {
            ShapeIndex = 0;
            _board = Enumerable
                .Range(0, 10000)
                .Select(_ => Enumerable
                    .Range(0, 7)
                    .Select(_ => false)
                    .ToArray())
                .ToList();
            _curr = new Shape(0, 0);
        }

        public int GetHeight()
        {
            return _board.Select((line, index) => line.Any(c => c) ? index + 1 : 0).Max();
        }

        public void Draw()
        {
            Console.Clear();
            // Console.WriteLine(new string('=', 20));
            for (var y = _board.Count - 1; y >= 0; y--)
            {
                for (var x = 0; x < _board[0].Length; x++)
                    Console.Write(_curr.Points.Contains(new Vector2Int(x, y))
                        ? '@'
                        : _board[y][x]
                            ? '#'
                            : '.');
                Console.WriteLine();
            }
        }

        private bool HasCollision(Vector2Int point)
        {
            return point.Y >= _board.Count || point.Y < 0
                                           || point.X >= _board[0].Length || point.X < 0 || _board[point.Y][point.X];
        }

        public void Tick(Direction? direction)
        {
            if (direction is not null) _curr.Move(direction.Value, HasCollision);
            var downMoveCollision = _curr.Move(Direction.Down, HasCollision);

            if (downMoveCollision)
            {
                foreach (var point in _curr.Points) _board[point.Y][point.X] = true;

                ShapeIndex++;
                _curr = new Shape(ShapeIndex, GetHeight());
            }
        }
    }
}
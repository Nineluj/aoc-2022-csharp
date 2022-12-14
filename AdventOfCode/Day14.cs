using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day14 : CustomDirBaseDay
{
    private readonly string _input;

    public Day14()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var sim = new SandSimulator(_input);
        while (!sim.IsSimulationDone())
            // sim.Draw();
            sim.Tick();
        // Console.ReadKey();
        // Console.WriteLine();
        // Console.WriteLine("DONE!!");
        var result = sim.GetMaterialCount(Material.Sand);
        return result.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private enum Material
    {
        Sand = 'o',
        Stone = '#'
    }

    private class SandSimulator
    {
        public enum State
        {
            Running,
            Done
        }

        private readonly Dictionary<Vector2Int, Material> _collisionItems;
        private readonly Vector2Int _sandSpawnPos = new(500, 0);
        private readonly int _xMax;

        private readonly int _xMin;
        private readonly int _yMax;
        private Vector2Int _currentSand;

        private State _gameState;

        public SandSimulator(string input)
        {
            _gameState = State.Running;
            _currentSand = _sandSpawnPos;
            _xMin = int.MaxValue;
            _xMax = int.MinValue;
            _yMax = int.MinValue;
            _collisionItems = new Dictionary<Vector2Int, Material>();
            foreach (var point in ParseInput(input).SelectMany(x => x))
            {
                _collisionItems[point] = Material.Stone;
                if (point.X < _xMin) _xMin = point.X;
                if (point.X > _xMax) _xMax = point.X;
                if (point.Y > _yMax) _yMax = point.Y;
            }
        }

        private List<Vector2Int> GetPointsInSegments(IEnumerable<Vector2Int> segments)
        {
            var segmentList = segments.ToList();
            var points = new List<Vector2Int>();
            var curr = segmentList[0];

            foreach (var point in segmentList.Slice(1))
            {
                var toNext = (point - curr).ToManhattanUnitVector();
                while (curr != point)
                {
                    points.Add(curr);
                    curr += toNext;
                }
            }

            points.Add(segmentList[^1]);
            return points;
        }

        private IEnumerable<IEnumerable<Vector2Int>> ParseInput(string input)
        {
            return Utils.GetLines(input)
                .Select(line =>
                    line.Split(" -> ").Select(coordString =>
                    {
                        var parts = coordString.Split(',', 2);
                        return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
                    }))
                .Select(GetPointsInSegments);
        }

        public bool IsSimulationDone()
        {
            return _gameState == State.Done;
        }

        public void Tick()
        {
            if (IsSimulationDone()) return;
            var down = _currentSand + Vector2Int.DownPosY;
            var downRight = _currentSand + Vector2Int.DownPosY + Vector2Int.Right;
            var downLeft = _currentSand + Vector2Int.DownPosY - Vector2Int.Right;

            foreach (var candidate in new[] { down, downLeft, downRight })
                if (!_collisionItems.ContainsKey(candidate))
                {
                    _currentSand = candidate;
                    if (_currentSand.Y > _yMax)
                    {
                        _gameState = State.Done;
                        Console.WriteLine("DONE!");
                    }

                    return;
                }


            _collisionItems.Add(_currentSand, Material.Sand);
            _currentSand = _sandSpawnPos;
        }

        public int GetMaterialCount(Material m)
        {
            return _collisionItems.Count(pair => pair.Value.Equals(m));
        }

        public void Draw()
        {
            Console.Clear();

            for (var y = 0; y <= _yMax; y++)
            {
                for (var x = _xMin; x <= _xMax; x++)
                {
                    var coord = new Vector2Int(x, y);
                    if (coord.Equals(_sandSpawnPos)) Console.Write('+');
                    else if (_collisionItems.TryGetValue(coord, out var material)) Console.Write((char)material);
                    else if (coord.Equals(_currentSand)) Console.Write('o');
                    else Console.Write('.');
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Sand at: {_currentSand}");
        }
    }
}
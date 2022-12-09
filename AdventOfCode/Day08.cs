namespace AdventOfCode;

public sealed class Day08 : CustomDirBaseDay
{
    private readonly string _input;

    public Day08()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<Tree> GetTrees(string input)
    {
        var heightGrid = GetGrid(input);

        var yMax = heightGrid.Count;
        var xMax = heightGrid[0].Count;
        // create trees
        var treeGrid = heightGrid
            .Select((row, y) => row.Select((height, x) => new Tree(height, $"({x},{y})")).ToList())
            .ToList();

        // link trees
        for (var y = 0; y < heightGrid.Count; y++)
        for (var x = 0; x < heightGrid[0].Count; x++)
        {
            var t = treeGrid[y][x];
            if (x > 0) t.SetNeighbour(Direction.West, treeGrid[y][x - 1]);
            if (x < xMax - 1) t.SetNeighbour(Direction.East, treeGrid[y][x + 1]);
            if (y > 0) t.SetNeighbour(Direction.North, treeGrid[y - 1][x]);
            if (y < yMax - 1) t.SetNeighbour(Direction.South, treeGrid[y + 1][x]);
        }

        return treeGrid.SelectMany(l => l);
    }

    private List<List<int>> GetGrid(string input)
    {
        return Utils.GetLines(input)
            .Select(line => line
                .Select(c => c - '0').ToList())
            .ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var trees = GetTrees(_input);
        var result = trees
            .Where(t => t.CanSeeOutsideAnyDir());

        return new ValueTask<string>(result.Count().ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var trees = GetTrees(_input);
        var result = trees
            .Select(t => t.GetTreeOverviewScore())
            .Max();
        return new ValueTask<string>(result.ToString());
    }

    private static IEnumerable<Direction> GetAllDirections()
    {
        return Enum
            .GetValues(typeof(Direction))
            .Cast<Direction>();
    }

    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    private class Tree
    {
        private readonly int _height;
        private readonly string _location;
        private readonly Dictionary<Direction, Tree> _neighbours;

        public Tree(int height, string location)
        {
            _height = height;
            _neighbours = new Dictionary<Direction, Tree>();
            _location = location;
        }

        public override string ToString()
        {
            return $"Tree [height={_height}] [Pos={_location}]";
        }

        public void SetNeighbour(Direction d, Tree t)
        {
            _neighbours[d] = t;
        }

        private bool CanSeeOutsideInDirection(Direction d, int height)
        {
            return !_neighbours.ContainsKey(d) ||
                   (height > _neighbours[d]._height && _neighbours[d].CanSeeOutsideInDirection(d, height));
        }

        public bool CanSeeOutsideAnyDir()
        {
            return GetAllDirections().Any(d => CanSeeOutsideInDirection(d, _height));
        }

        public int CountVisibleTrees(Direction d, int height)
        {
            if (!_neighbours.ContainsKey(d)) return 0;
            var neighbour = _neighbours[d];
            if (neighbour._height < height) return 1 + neighbour.CountVisibleTrees(d, height);
            return 1;
        }

        public int GetTreeOverviewScore()
        {
            return GetAllDirections()
                .Select(d => CountVisibleTrees(d, _height))
                .Aggregate((a, b) => a * b);
        }
    }
}
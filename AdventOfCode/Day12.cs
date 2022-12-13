using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day12 : CustomDirBaseDay
{
    private const char StartChar = (char)2017;
    private const char EndChar = (char)2012;
    private readonly string _input;

    public Day12()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    /// <summary>
    ///     Parse the input to produce a directed graph of Tiles
    ///     that are only connected (A -> B) if you can walk from
    ///     Tile A to tile B.
    /// </summary>
    private static List<List<Tile>> ParseTileMap(string input)
    {
        var lines = Utils.GetLines(input).ToList();
        var tileMap = lines
            .Select((l, y) =>
                l.Select((c, x) => new Tile(x, y, c switch
                {
                    'S' => StartChar,
                    'E' => EndChar,
                    _ => c
                })).ToList()).ToList();
        for (var y = 0; y < lines.Count; y++)
        for (var x = 0; x < lines[0].Length; x++)
        {
            var tile = tileMap[y][x];
            if (y > 0) tile.SetConnection(CardinalDirection.North, tileMap[y - 1][x]);
            if (y < tileMap.Count - 1) tile.SetConnection(CardinalDirection.South, tileMap[y + 1][x]);
            if (x > 0) tile.SetConnection(CardinalDirection.West, tileMap[y][x - 1]);
            if (x < tileMap[0].Count - 1) tile.SetConnection(CardinalDirection.East, tileMap[y][x + 1]);
        }

        return tileMap;
    }

    private IEnumerable<Tile> BreathFirstSearch(Tile start, Func<Tile, bool> isEndFn, Func<Tile, Tile, bool> canMoveFn)
    {
        var initialPaths = new LinkedList<Tile>(new List<Tile> { start });
        var queue = new Queue<LinkedList<Tile>>(new List<LinkedList<Tile>> { initialPaths });
        var seen = new HashSet<Tile>();

        while (queue.Any())
        {
            var path = queue.Dequeue();
            var node = path.Last();
            if (isEndFn(node)) return path;

            foreach (var (_, adjacent) in node.Connections)
            {
                if (!canMoveFn(node, adjacent)) continue;
                if (seen.Contains(adjacent)) continue;
                seen.Add(adjacent);
                var newPath = new LinkedList<Tile>(path);
                newPath.AddLast(adjacent);
                queue.Enqueue(newPath);
            }
        }

        throw new SolvingException("Couldn't find the path to the end");
    }

    private static bool CanMoveTo(Tile from, Tile to)
    {
        return from.Height == StartChar
               || (from.Height == 'z' && to.Height == EndChar)
               || from.Height + 1 >= to.Height;
    }

    private IEnumerable<Tile> BreathFirstSearchToEnd(Tile start)
    {
        bool IsEndFn(Tile t)
        {
            return t.Height == EndChar;
        }

        return BreathFirstSearch(start, IsEndFn, CanMoveTo);
    }

    private IEnumerable<Tile> BreathFirstSearchToA(Tile endTile)
    {
        bool IsEndFn(Tile t)
        {
            return t.Height == 'a';
        }

        bool CanMoveToReverse(Tile a, Tile b)
        {
            return CanMoveTo(b, a);
        }

        // Run a BFS from the end to the first Tile with height 'a' using the
        // reversed version of the canMoveTo function
        return BreathFirstSearch(endTile, IsEndFn, CanMoveToReverse);
    }

    public override ValueTask<string> Solve_1()
    {
        var map = ParseTileMap(_input).SelectMany(t => t).First(t => t.Height == StartChar);
        var bestPath = BreathFirstSearchToEnd(map).ToList();
        var result = bestPath.Count - 1;
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var endTile = ParseTileMap(_input).SelectMany(t => t).First(t => t.Height == EndChar);
        var bestPath = BreathFirstSearchToA(endTile).ToList();
        var result = bestPath.Count - 1;
        return new ValueTask<string>(result.ToString());
    }

    private class Tile
    {
        public readonly Dictionary<CardinalDirection, Tile> Connections;
        public readonly char Height;
        public readonly string id;

        public Tile(int x, int y, char height)
        {
            id = $"({x},{y})";
            Height = height;
            Connections = new Dictionary<CardinalDirection, Tile>();
        }

        public void SetConnection(CardinalDirection d, Tile t)
        {
            Connections[d] = t;
        }
    }
}
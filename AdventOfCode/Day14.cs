using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day14 : CustomDirBaseDay
{
    public enum Material
    {
        Sand = 'o',
        Stone = '#'
    }

    private readonly string _input;
    private readonly Vector2Int _sandStartingPosition = new(500, 0);

    public Day14()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var game = Create(_input, false);
        RunToCompletion(game, (state, coord) =>
            coord.Y > state.YMax
        );
        var result = GetMaterialCount(game, Material.Sand);
        return ValueTask.FromResult(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var game = Create(_input, false);
        AddFloorSegment(game);
        try
        {
            // rely on catching an exception here instead of checking for a condition
            // since the method will error when the initial (desired) spot is filled
            RunToCompletion(game, (_, _) => false);
        }
        catch (InvalidOperationException)
        {
        }

        var result = GetMaterialCount(game, Material.Sand);
        return ValueTask.FromResult(result.ToString());
    }

    private static int GetMaterialCount(GameState state, Material m)
    {
        return state.CollisionMaterials.Count(pair => pair.Value == m);
    }

    private static List<Vector2Int> GetPointsInSegments(IEnumerable<Vector2Int> segments)
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

    private static IEnumerable<IEnumerable<Vector2Int>> ParseInput(string input)
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

    private static void AddFloorSegment(GameState state)
    {
        var segmentStart = new Vector2Int(state.XMin - 500, state.YMax + 2);
        var segmentEnd = new Vector2Int(state.XMax + 500, state.YMax + 2);
        foreach (var point in GetPointsInSegments(new List<Vector2Int> { segmentStart, segmentEnd }))
            state.CollisionMaterials.Add(point, Material.Stone);
    }

    private GameState Create(string input, bool addFloor)
    {
        var xMin = int.MaxValue;
        var xMax = int.MinValue;
        var yMax = int.MinValue;
        var collisionItems = new Dictionary<Vector2Int, Material>();
        foreach (var point in ParseInput(input).SelectMany(x => x))
        {
            collisionItems[point] = Material.Stone;
            if (point.X < xMin) xMin = point.X;
            if (point.X > xMax) xMax = point.X;
            if (point.Y > yMax) yMax = point.Y;
        }

        var state = new GameState(collisionItems, xMin, xMax, yMax);
        if (addFloor) AddFloorSegment(state);
        return state;
    }

    private void RunToCompletion(GameState initial, Func<GameState, Vector2Int, bool> completedFn)
    {
        var state = initial;
        var validPositions = new LinkedList<Vector2Int>();
        validPositions.AddLast(_sandStartingPosition);
        var curr = _sandStartingPosition + Vector2Int.PosY;

        while (true)
        {
            if (completedFn(state, curr)) return;

            var down = curr + Vector2Int.PosY;
            var downRight = down + Vector2Int.PosX;
            var downLeft = down - Vector2Int.PosX;

            var foundMatch = false;
            foreach (var candidate in new[] { down, downLeft, downRight })
                if (!state.CollisionMaterials.ContainsKey(candidate))
                {
                    validPositions.AddLast(curr);
                    curr = candidate;
                    foundMatch = true;
                    break;
                }

            if (!foundMatch)
            {
                state.CollisionMaterials.Add(curr, Material.Sand);
                // if we can't move anywhere, mark the current spot as a collision spot
                // and continue from the last valid position
                curr = validPositions.Last();
                validPositions.RemoveLast();
            }

            // Draw(state, curr);
            // Console.WriteLine(curr);
        }
    }

    public void Draw(GameState gs, Vector2Int current)
    {
        Console.Clear();

        for (var y = 0; y <= gs.YMax; y++)
        {
            for (var x = gs.XMin; x <= gs.XMax; x++)
            {
                var coord = new Vector2Int(x, y);
                if (coord.Equals(_sandStartingPosition)) Console.Write('+');
                else if (gs.CollisionMaterials.TryGetValue(coord, out var material)) Console.Write((char)material);
                else if (coord.Equals(current)) Console.Write('$');
                else Console.Write('.');
            }

            Console.WriteLine();
        }

        Console.WriteLine($"Currently at {current}");
    }

    public record GameState(
        Dictionary<Vector2Int, Material> CollisionMaterials,
        int XMin, int XMax, int YMax);
}
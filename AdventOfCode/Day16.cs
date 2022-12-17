using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day16 : CustomDirBaseDay
{
    private readonly string _input;

    public Day16()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private Dictionary<string, Valve> ParseInput(string input)
    {
        var lineRe = new Regex(
            @"^Valve (?<name>\w+) has flow rate=(?<flow>\d+);.* to valves? (?:(?:,\s)?(?<valves>\w+))+$");

        var valveConnections = new Dictionary<Valve, string[]>();

        var valves = Utils.GetLines(input)
            .Select(line =>
            {
                var m = lineRe.Match(line);
                var newValve = new Valve(m.Groups["name"].Value, int.Parse(m.Groups["flow"].Value));
                valveConnections.Add(newValve, m.Groups["valves"].Captures.Select(c => c.Value).ToArray());
                return newValve;
            })
            .ToDictionary(valve => valve.Name, valve => valve);

        foreach (var valve in valveConnections)
        foreach (var conn in valve.Value)
            valve.Key.AddConnection(valves[conn]);

        foreach (var pair in valves) pair.Value.ComputeDistanceToOthers();

        return valves;
    }

    // private static int FindMaxPressureReleaseRecursive(Valve curr, int minutesLeft, HashSet<string> toggledValves)
    // {
    // }

    private int FindMaxPressureRelease(Valve start, int maxTime)
    {
        var time = 0;
        var totalPressureReleased = 0;
        var pressureReleasedPerMinute = 0;
        var curr = start;

        var nonZeroPressureValves = start.DistanceToOthers.Count(pair => pair.Key.FlowRate != 0);
        Console.WriteLine($"Started at {curr.Name}");

        var unlockedValves = new HashSet<Valve>();
        while (time <= maxTime && unlockedValves.Count < nonZeroPressureValves) // check this for an off by one later
        {
            var bestCandidate = curr.DistanceToOthers.MaxBy(pair =>
                unlockedValves.Contains(pair.Key)
                    ? 0
                    : pair.Key.PressureReleasedForRemainingTime(maxTime - time - pair.Value) / pair.Value);
            Console.WriteLine($"Going to {bestCandidate.Key.Name}");

            // the time spent to release the candidate is the time it takes to get to it
            // plus one for the time to release it
            var minutesElapsedToNext = bestCandidate.Value + 1;
            curr = bestCandidate.Key;
            unlockedValves.Add(curr);

            totalPressureReleased += pressureReleasedPerMinute * minutesElapsedToNext;
            Console.WriteLine($"Released {pressureReleasedPerMinute} * {minutesElapsedToNext}");
            time += minutesElapsedToNext;
            pressureReleasedPerMinute += curr.FlowRate;
        }

        if (time <= maxTime) totalPressureReleased += pressureReleasedPerMinute * (maxTime - time);

        return totalPressureReleased;
    }

    public override ValueTask<string> Solve_1()
    {
        var valves = ParseInput(_input);
        var result = FindMaxPressureRelease(
            valves.First(pair => pair.Key.Equals("AA")).Value,
            30);
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    public class Valve
    {
        public readonly int FlowRate;
        public List<Valve> Connections;
        public Dictionary<Valve, int> DistanceToOthers;
        public string Name;

        public Valve(string name, int flowRate)
        {
            Name = name;
            Connections = new List<Valve>();
            DistanceToOthers = new Dictionary<Valve, int>();
            FlowRate = flowRate;
        }

        public void AddConnection(Valve other)
        {
            Connections.Add(other);
        }

        public void ComputeDistanceToOthers()
        {
            // bfs from this node to others
            var queue = new Queue<Valve>();
            var seen = new HashSet<Valve>();
            seen.Add(this);
            queue.Enqueue(this);
            DistanceToOthers[this] = 0;
            while (queue.Any())
            {
                var curr = queue.Dequeue();

                foreach (var conn in curr.Connections.Where(conn => !seen.Contains(conn)))
                {
                    seen.Add(conn);
                    DistanceToOthers[conn] = DistanceToOthers[curr] + 1;
                    queue.Enqueue(conn);
                }
            }

            DistanceToOthers.Remove(this);
        }

        public int PressureReleasedForRemainingTime(int minutes)
        {
            return FlowRate * minutes;
        }
    }
}
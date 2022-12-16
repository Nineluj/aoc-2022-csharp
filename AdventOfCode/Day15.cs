using System.Text.RegularExpressions;
using AdventOfCode.Models;

namespace AdventOfCode;

public sealed class Day15 : CustomDirBaseDay
{
    private readonly string _input;

    public Day15()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private ScanData ParseInput(string input)
    {
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var lineRe = new Regex(@"Sensor at x=(-?\d+), y=(-?\d+):.*at x=(-?\d+), y=(-?\d+)");
        var sensorRanges = Utils.GetLines(input)
            .Select(line => Utils.GetAllRegexMatches(lineRe, line))
            .Select(elems => elems.Select(int.Parse))
            .Select(elems =>
            {
                var l = elems.ToList();
                var sensor = new Vector2Int(l[0], l[1]);
                var beacon = new Vector2Int(l[2], l[3]);
                minX = Math.Min(Math.Min(minX, sensor.X), beacon.X);
                maxX = Math.Max(Math.Max(maxX, sensor.X), beacon.X);
                return new SensorRange(sensor, sensor.ManhattanDistanceTo(beacon));
            }).ToList();
        return new ScanData(sensorRanges, minX, maxX);
    }

    private static List<Interval> MergeIntervals(List<Interval> lst)
    {
        lst.Sort((a, b) => a.Start - b.Start);

        var stack = new Stack<Interval>();
        stack.Push(lst[0]);

        for (var i = 1; i < lst.Count; i++)
        {
            var curr = lst[i];
            var top = stack.Peek();

            if (top.End < curr.Start)
            {
                stack.Push(curr);
            }
            else if (top.End < curr.End)
            {
                stack.Pop();
                stack.Push(new Interval(top.Start, curr.End));
            }
        }

        return stack.ToList();
    }

    private int CountPossibleBeaconPositions(ScanData scanData, int scanY)
    {
        var intervals = scanData.SensorRanges
            .Select(x =>
            {
                var distToScanLevel = x.Sensor.ManhattanDistanceTo(x.Sensor with { Y = scanY });
                var rangeRemaining = x.Range - distToScanLevel;

                if (rangeRemaining < 0) return new Interval(0, 0);

                var xMin = x.Sensor.X - rangeRemaining;
                var xMax = x.Sensor.X + rangeRemaining;
                return new Interval(xMin, xMax);
            })
            .Where(x => !x.IsZero())
            .ToList();

        var mergedIntervals = MergeIntervals(intervals);
        return mergedIntervals.Select(x => x.CountRange()).Sum();
    }

    public override ValueTask<string> Solve_1()
    {
        var scanData = ParseInput(_input);
        var result = CountPossibleBeaconPositions(scanData, 2_000_000);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private record Interval(int Start, int End)
    {
        public bool IsZero()
        {
            return Start == 0 && End == 0;
        }

        public int CountRange()
        {
            return End - Start;
        }
    }


    private record SensorRange(Vector2Int Sensor, int Range);

    private record ScanData(IEnumerable<SensorRange> SensorRanges, int MinX, int MaxX);
}
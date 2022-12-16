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

    private static IEnumerable<SensorRange> ParseInput(string input)
    {
        var lineRe = new Regex(@"Sensor at x=(-?\d+), y=(-?\d+):.*at x=(-?\d+), y=(-?\d+)");
        return Utils.GetLines(input)
            .Select(line => Utils.GetAllRegexMatches(lineRe, line))
            .Select(elems => elems.Select(int.Parse))
            .Select(elems =>
            {
                var l = elems.ToList();
                var sensor = new Vector2Int(l[0], l[1]);
                var beacon = new Vector2Int(l[2], l[3]);
                return new SensorRange(sensor, sensor.ManhattanDistanceTo(beacon));
            }).ToList();
    }

    private static IEnumerable<Interval> MergeIntervals(List<Interval> lst)
    {
        lst.Sort((a, b) => a.Start - b.Start);
        var stack = new Stack<Interval>();
        stack.Push(lst[0]);

        for (var i = 1; i < lst.Count; i++)
        {
            var curr = lst[i];
            var top = stack.Peek();

            // had to update the range checking here
            // since we want to merge ie [[0,11] [12,24]]
            if (top.End + 1 < curr.Start)
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

    private static IEnumerable<Interval> GetMergedIntervals(IEnumerable<SensorRange> scanData, int fixedScanVal,
        bool horizontal)
    {
        var intervals = scanData
            .Select(sensor =>
            {
                var pointOnScanLevelLine = horizontal
                    ? sensor.Position with { Y = fixedScanVal }
                    : sensor.Position with { X = fixedScanVal };
                var distToScanLevel = sensor.Position.ManhattanDistanceTo(pointOnScanLevelLine);
                var rangeRemaining = sensor.Range - distToScanLevel;
                if (rangeRemaining < 0) return new Interval(0, 0);
                var variableVal = horizontal ? sensor.Position.X : sensor.Position.Y;
                var min = variableVal - rangeRemaining;
                var max = variableVal + rangeRemaining;
                return new Interval(min, max);
            })
            .Where(x => !x.IsZero())
            .ToList();
        return MergeIntervals(intervals);
    }

    private int CountPossibleBeaconPositions(IEnumerable<SensorRange> scanData, int fixedScanVal, bool horizontal)
    {
        var intervals = GetMergedIntervals(scanData, fixedScanVal, horizontal);
        return intervals.Select(x => x.CountRange()).Sum();
    }

    private int CountPossibleBeaconPositionsWithLimit(IEnumerable<SensorRange> scanData, int fixedScanVal,
        Interval allowed,
        bool horizontal)
    {
        var intervals = GetMergedIntervals(scanData, fixedScanVal, horizontal);
        return intervals.Select(interval =>
            new Interval(
                Math.Max(allowed.Start, interval.Start),
                Math.Min(allowed.End, interval.End)).CountRange()
        ).Sum();
    }

    private long FindTuningFrequency(IReadOnlyCollection<SensorRange> scanData, Interval possibleX,
        Interval possibleY)
    {
        var expectedHorizontalCount = possibleX.CountRange();
        var y = (long)Enumerable.Range(possibleY.Start, possibleY.End)
            .First(y =>
                CountPossibleBeaconPositionsWithLimit(scanData, y, possibleX, true) < expectedHorizontalCount);

        var expectedVerticalCount = possibleX.CountRange();
        var x = (long)Enumerable.Range(possibleX.Start, possibleX.End)
            .First(x =>
                CountPossibleBeaconPositionsWithLimit(scanData, x, possibleY, false) < expectedVerticalCount);

        return x * 4_000_000 + y;
    }

    public override ValueTask<string> Solve_1()
    {
        var scanData = ParseInput(_input).ToList();
        var result = CountPossibleBeaconPositions(scanData, 2_000_000, true);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var scanData = ParseInput(_input).ToList();
        var result = FindTuningFrequency(
            scanData,
            new Interval(0, 4_000_000),
            new Interval(0, 4_000_000)
        );
        return new ValueTask<string>(result.ToString());
    }

    private record SensorRange(Vector2Int Position, int Range);
}
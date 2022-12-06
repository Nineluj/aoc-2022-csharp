using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day04 : CustomDirBaseDay
{
    private readonly string _input;

    public Day04()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private record Range(int Start, int End);
    private record RangePair(Range First, Range Second);
    
    private static IEnumerable<RangePair> GetPairRanges(string input)
    {
        var re = new Regex(@"(\d+)-(\d+),(\d+)-(\d+)");
        return Utils.GetLines(input)
            .Select(line =>
            {
                var x = re.Match(line).Groups.Values
                    // the first group is the full capture, skip that
                    .Skip(1)
                    .Select(v => int.Parse(v.Value))
                    .ToList();
                return new RangePair(new Range(x[0], x[1]), new Range(x[2], x[3]));
            })
            .ToList();
    }

    private static bool HasFullOverlap(RangePair rangePairs)
    {
        return (rangePairs.First.Start >= rangePairs.Second.Start &&
                rangePairs.First.End <= rangePairs.Second.End) ||
               (rangePairs.First.Start <= rangePairs.Second.Start &&
                rangePairs.First.End >= rangePairs.Second.End);
    }
    
    private static bool HasPartialOverlap(RangePair rangePairs)
    {
        // just need to check if first range's start or end are inside of the
        // range of the second range OR if there's a full overlap
        return HasFullOverlap(rangePairs) ||
               (rangePairs.First.Start >= rangePairs.Second.Start && rangePairs.First.Start <= rangePairs.Second.End) ||
               (rangePairs.First.End >= rangePairs.Second.Start && rangePairs.First.End <= rangePairs.Second.End);
    }

    public override ValueTask<string> Solve_1()
    {
        var ranges = GetPairRanges(_input);
        var result = ranges.Where(HasFullOverlap).Count();
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var ranges = GetPairRanges(_input);
        var result = ranges.Where(HasPartialOverlap).Count();
        return new ValueTask<string>(result.ToString());
    }
}
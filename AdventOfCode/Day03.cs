using System.Diagnostics;

namespace AdventOfCode;

public sealed class Day03 : CustomDirBaseDay
{
    private readonly string _input;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1()
    {
        var rucksacks = GetRucksacks(_input);
        var val= rucksacks
            .Select(FindShared)
            .Select(GetPriority)
            .Sum();
            
        return new ValueTask<string>(val.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var rucksacks = GetRucksacks(_input);

        var val = rucksacks
            .Chunk(3)
            .Select(sacks =>
            {
                return sacks
                    .Select(s => s.Compartment1.Union(s.Compartment2))
                    .Aggregate((a, b) => a.Intersect(b)).First();
            })
            .Select(GetPriority)
            .Sum();
        
        return new ValueTask<string>(val.ToString());
    }

    private static char FindShared(Rucksack r)
    {
        return r.Compartment1.Where(r.Compartment2.Contains).First();
    }

    private static int GetPriority(char c)
    {
        return c switch
        {
            >= 'A' and <= 'Z' => c - 'A' + 26 + 1,
            >= 'a' and <= 'z' => c - 'a' + 1,
            _ => throw new ArgumentException($"Cannot get priority of non-letter {c}")
        };
    }
    
    private record Rucksack(HashSet<char> Compartment1, HashSet<char> Compartment2);

    private List<Rucksack> GetRucksacks(string input)
    {
        return Utils.GetLines(input).Select(line =>
        {
            var halfPoint = line.Length / 2;
            return new Rucksack(new HashSet<char>(line[..halfPoint]), new HashSet<char>(line[halfPoint..]));
        }).ToList();
    }
}
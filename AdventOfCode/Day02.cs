using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string _input;

    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var games = ParseGames(_input);
        return new ValueTask<string>(games.Select(GetGameScore).Sum().ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var games = ParseGames(_input);
        return new ValueTask<string>(games.Select(GetGameScoreV2).Sum().ToString());
    }

    private static int GetGameScore(RockPaperScissorsRound round)
    {
        var moveScore = round.c2 - 'X' + 1;
        var resultScore = Utils.Mod(round.c1 - round.c2 + ('X' - 'A'), 3) switch
        {
            0 => 3,
            1 => 0,
            2 => 6,
            {} other => throw new ArgumentOutOfRangeException($"Can't deal with {other}")
        };
        return moveScore + resultScore;
    }

    private char GetExpectedMove(RockPaperScissorsRound round)
    {
        return round.c2 switch
        {
            'X' => round.c1 switch
            {
                'A' => 'Z',
                'B' => 'X',
                'C' => 'Y',
                _ => throw new ArgumentOutOfRangeException()
            },
            'Y' => (char)('X' + (round.c1 - 'A')),
            'Z' => round.c1 switch
            {
                'A' => 'Y',
                'B' => 'Z',
                'C' => 'X',
                _ => throw new ArgumentOutOfRangeException()
            },
            { } other => throw new ArgumentOutOfRangeException($"Can't deal with {other}")
        };
    }

    private int GetGameScoreV2(RockPaperScissorsRound round)
    {
        var expectedMove = GetExpectedMove(round);
        return GetGameScore(round with { c2 = expectedMove });
    }
    
    private record RockPaperScissorsRound(char c1, char c2);
    private List<RockPaperScissorsRound> ParseGames(string input)
    {
        var re = new Regex("([A-C]) ([X-Z])");
        return (
            from line in input.Split(Environment.NewLine) 
            select re.Match(line) into match 
            let move1 = match.Groups[1].Value.First()
            let move2 = match.Groups[2].Value.First()
            select new RockPaperScissorsRound(move1, move2)
            ).ToList();
    }
}

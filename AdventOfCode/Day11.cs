using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day11 : CustomDirBaseDay
{
    private readonly string _input;

    public Day11()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    // I had to bump this to long instead of int since even with the modulo
    // trick the square was causing an overflow for int(32)
    private Func<long, long> GetOperation(List<string> operationData)
    {
        if (operationData[1] == "old")
            return operationData[0] switch
            {
                "+" => a => a + a,
                "*" => a => a * a,
                _ => throw new ArgumentOutOfRangeException()
            };

        var constant = int.Parse(operationData[1]);
        return operationData[0] switch
        {
            "+" => a => a + constant,
            "*" => a => a * constant,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private List<Monkey> ParseMonkeys(string input)
    {
        var lines = Utils.GetLinesIncludeEmpty(input).ToList();

        var monkeyNameRe = new Regex(@"Monkey (\S+):$");
        var startingItemsRe = new Regex(@"Starting items: (.*)$");
        var operationRe = new Regex(@"Operation: new = old (\S) (\S+)$");
        var testRe = new Regex(@"Test: divisible by (\d+)$");
        var throwTargetRe = new Regex(@"throw to monkey (\d+)$");

        var monkeys = new List<Monkey>();
        // tuple is <divisible by _, throw to _, throw to _>
        var monkeyTests = new List<Tuple<int, int, int>>();

        for (var i = 0; i < lines.Count; i += 7)
        {
            var name = Utils.GetFirstMatchRegex(monkeyNameRe, lines[i]);
            var startItems = Utils.GetFirstMatchRegex(startingItemsRe, lines[i + 1])
                .Split(", ").Select(x => new Item(int.Parse(x)));
            var operationData = Utils.GetAllRegexMatches(operationRe, lines[i + 2])
                .ToList();
            var operation = GetOperation(operationData);
            monkeys.Add(new Monkey(name, startItems, operation));

            // parse monkey test data
            var testDivisor = int.Parse(Utils.GetFirstMatchRegex(testRe, lines[i + 3]));
            var trueTargetInt = int.Parse(Utils.GetFirstMatchRegex(throwTargetRe, lines[i + 4]));
            var falseTargetInt = int.Parse(Utils.GetFirstMatchRegex(throwTargetRe, lines[i + 5]));
            monkeyTests.Add(new Tuple<int, int, int>(testDivisor, trueTargetInt, falseTargetInt));
        }

        // add monkey tests
        for (var i = 0; i < monkeyTests.Count; i++)
        {
            var test = monkeyTests[i];
            monkeys[i].SetThrowPreference(new ThrowPreference(
                x => x % test.Item1 == 0,
                test.Item1,
                monkeys[test.Item2],
                monkeys[test.Item3]
            ));
        }

        return monkeys;
    }

    private BigInteger CalculateMonkeyBusiness(List<Monkey> monkeys, int numRounds, bool isCalmedAfterHandling)
    {
        var monkeyTestLCM = monkeys
            .Select(m => m.ThrowPreference.DivisibleByNumber)
            .Aggregate((a, b) => a * b);

        for (var i = 0; i < numRounds; i++)
            foreach (var monkey in monkeys)
                monkey.TakeTurn(isCalmedAfterHandling, monkeyTestLCM);

        return monkeys
            .Select(m => new BigInteger(m.InspectionCount))
            .OrderByDescending(x => x)
            .Take(2)
            .Aggregate((a, b) => a * b);
    }

    public override ValueTask<string> Solve_1()
    {
        var monkeys = ParseMonkeys(_input);
        var result = CalculateMonkeyBusiness(monkeys, 20, true);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var monkeys = ParseMonkeys(_input);
        var result = CalculateMonkeyBusiness(monkeys, 10_000, false);
        return new ValueTask<string>(result.ToString());
    }

    private class Item
    {
        public long WorryLevel;

        public Item(int worryLevel)
        {
            WorryLevel = worryLevel;
        }

        public void HandleMonkeyHandling(Func<long, long> operation, bool isCalmedAfterHandling, int monkeyTestLCM)
        {
            var duringHandling = operation(WorryLevel);
            var newWorryLevel = isCalmedAfterHandling
                ? Convert.ToInt32(Math.Floor(duringHandling / 3.0))
                : duringHandling % monkeyTestLCM;
            WorryLevel = newWorryLevel;
        }
    }

    private record ThrowPreference(Func<long, bool> Cond, int DivisibleByNumber, Monkey TrueTarget, Monkey FalseTarget);

    private class Monkey
    {
        private readonly Queue<Item> _items;
        private readonly Func<long, long> _operation;
        public readonly string Name;

        public int InspectionCount;
        public ThrowPreference ThrowPreference;

        public Monkey(
            string name,
            IEnumerable<Item> startItems,
            Func<long, long> operation)
        {
            Name = name;
            _items = new Queue<Item>(startItems);
            _operation = operation;
        }

        public void SetThrowPreference(ThrowPreference throwPreference)
        {
            ThrowPreference = throwPreference;
        }

        private void ReceiveItem(Item item)
        {
            _items.Enqueue(item);
        }

        public void TakeTurn(bool isCalmedAfterHandling, int monkeyTestLCM)
        {
            // inspect items
            foreach (var item in _items)
            {
                item.HandleMonkeyHandling(_operation, isCalmedAfterHandling, monkeyTestLCM);
                InspectionCount++;
            }

            // throw items
            while (_items.Any())
            {
                var item = _items.Dequeue();
                var target = ThrowPreference.Cond(item.WorryLevel)
                    ? ThrowPreference.TrueTarget
                    : ThrowPreference.FalseTarget;
                target.ReceiveItem(item);
            }
        }
    }
}
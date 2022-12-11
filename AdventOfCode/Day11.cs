using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day11 : CustomDirBaseDay
{
    private readonly string _input;

    public Day11()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private Func<int, int> GetOperation(List<string> operationData)
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
                monkeys[test.Item2],
                monkeys[test.Item3]
            ));
        }

        return monkeys;
    }

    private int CalculateMonkeyBusiness(List<Monkey> monkeys, int numRounds)
    {
        for (var i = 0; i < numRounds; i++)
            foreach (var monkey in monkeys)
                monkey.TakeTurn();

        return monkeys.Select(m => m.InspectionCount).OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b);
    }

    public override ValueTask<string> Solve_1()
    {
        var monkeys = ParseMonkeys(_input);
        var result = CalculateMonkeyBusiness(monkeys, 20);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private class Item
    {
        public int WorryLevel;

        public Item(int worryLevel)
        {
            WorryLevel = worryLevel;
        }

        public void HandleMonkeyHandling(Func<int, int> operation)
        {
            var duringHandling = operation(WorryLevel);
            var afterHandling = Convert.ToInt32(Math.Floor(duringHandling / 3.0));
            WorryLevel = afterHandling;
        }
    }

    private record ThrowPreference(Func<int, bool> Cond, Monkey TrueTarget, Monkey FalseTarget);

    private class Monkey
    {
        private readonly Queue<Item> _items;
        private readonly Func<int, int> _operation;
        private string _monkeyId;
        private ThrowPreference _throwPreference;

        public int InspectionCount;

        public Monkey(
            string monkeyId,
            IEnumerable<Item> startItems,
            Func<int, int> operation)
        {
            _items = new Queue<Item>(startItems);
            _operation = operation;
            _monkeyId = monkeyId;
        }

        public void SetThrowPreference(ThrowPreference throwPreference)
        {
            _throwPreference = throwPreference;
        }

        private void ReceiveItem(Item item)
        {
            _items.Enqueue(item);
        }

        public void TakeTurn()
        {
            // inspect items
            foreach (var item in _items)
            {
                item.HandleMonkeyHandling(_operation);
                InspectionCount++;
            }

            // throw items
            while (_items.Any())
            {
                var item = _items.Dequeue();
                var target = _throwPreference.Cond(item.WorryLevel)
                    ? _throwPreference.TrueTarget
                    : _throwPreference.FalseTarget;
                target.ReceiveItem(item);
            }
        }
    }
}
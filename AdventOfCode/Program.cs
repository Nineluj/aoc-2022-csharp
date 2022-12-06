using AdventOfCode;
using System.CommandLine;

var allOption = new Option<bool>(
    name: "--all",
    description: "Run the solutions for all the days");
var testOption = new Option<bool>(
    name: "--test",
    description: "Run using the test data in the TestInputs/ directory");
var dayOption = new Option<List<uint>>(
    "--day",
    "Specify what days to run"
) { Arity = ArgumentArity.ZeroOrMore };

var rootCommand = new RootCommand("Command for running AOC solutions");
rootCommand.AddOption(allOption);
rootCommand.AddOption(testOption);
rootCommand.AddOption(dayOption);

rootCommand.SetHandler(async (runAll, useTestData, days) => 
    {
        if (useTestData)
        {
            SettingsSingleton.Instance.InputFileDirPath = "TestInputs";
        }

        if (days.Any())
        { 
            await Solver.Solve(days);
            
        } else if (runAll)
        {
            await Solver.SolveAll(opt =>
            {
                opt.ShowConstructorElapsedTime = true;
                opt.ShowTotalElapsedTimePerDay = true;
            });
        }
        else
        {
            await Solver.SolveLast(opt => opt.ClearConsole = false);
        }
    }, allOption, testOption, dayOption);

rootCommand.Invoke(args);
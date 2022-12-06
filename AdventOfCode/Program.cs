using System.CommandLine;
using AdventOfCode;

var allOption = new Option<bool>(
    "--all",
    "Run the solutions for all the days");
var testOption = new Option<bool>(
    "--test",
    "Run using the test data in the TestInputs/ directory");
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
    if (useTestData) SettingsSingleton.Instance.InputFileDirPath = "TestInputs";

    if (days.Any())
        await Solver.Solve(days);
    else if (runAll)
        await Solver.SolveAll(opt =>
        {
            opt.ShowConstructorElapsedTime = true;
            opt.ShowTotalElapsedTimePerDay = true;
        });
    else
        await Solver.SolveLast(opt => opt.ClearConsole = false);
}, allOption, testOption, dayOption);

#nullable enable
var fetchInputCommand = new Command("download");

var sessionTokenOption = new Option<string?>("--session") { IsRequired = false };
var yearOption = new Option<uint>(
    "--year", () => 2022, "The year to run");
fetchInputCommand.AddOption(sessionTokenOption);
fetchInputCommand.AddOption(dayOption);
fetchInputCommand.AddOption(yearOption);
fetchInputCommand.SetHandler(InputDownloader.Download,
    sessionTokenOption, dayOption, yearOption);

rootCommand.AddCommand(fetchInputCommand);
await rootCommand.InvokeAsync(args);
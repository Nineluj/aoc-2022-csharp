# Nineluj's AOC 2022

I'm using C# this year. The repo is based off [AdventOfCode.Template](https://github.com/eduherminio/AdventOfCode.Template) with some
modifications to allow test cases to be run.

Uses `System.CommandLine`. Options are:
```shell
--all # run all days
--test # use test data from TestInputs
--day <specific day> # runs the given days
<? nothing> # runs the last day
```

Download inputs using:
```shell
download --session <SESSION_TOKEN> --day 5
```
The day is optional and the session token can also be set as an
environment variable.
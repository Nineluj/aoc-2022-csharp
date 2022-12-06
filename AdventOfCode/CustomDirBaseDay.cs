namespace AdventOfCode;

public abstract class CustomDirBaseDay : BaseDay
{
    // protected override string ClassPrefix { get; } = "Day";
    
    private string TestInputFileDirPath { get; set; }
    protected override string InputFileDirPath => TestInputFileDirPath;

    protected CustomDirBaseDay()
    {
        TestInputFileDirPath = SettingsSingleton.Instance.InputFileDirPath;
    }
}
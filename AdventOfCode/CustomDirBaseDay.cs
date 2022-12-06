namespace AdventOfCode;

public abstract class CustomDirBaseDay : BaseDay
{
    private string TestInputFileDirPath { get; }
    protected override string InputFileDirPath => TestInputFileDirPath;

    protected CustomDirBaseDay()
    {
        TestInputFileDirPath = SettingsSingleton.Instance.InputFileDirPath;
    }
}
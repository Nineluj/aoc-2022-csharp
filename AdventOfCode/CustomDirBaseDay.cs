namespace AdventOfCode;

public abstract class CustomDirBaseDay : BaseDay
{
    protected CustomDirBaseDay()
    {
        TestInputFileDirPath = SettingsSingleton.Instance.InputFileDirPath;
    }

    private string TestInputFileDirPath { get; }
    protected override string InputFileDirPath => TestInputFileDirPath;
}
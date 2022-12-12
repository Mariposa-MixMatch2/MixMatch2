namespace MixMatch2.Shared.Interfaces;

public interface ITest
{
    public Task<TestResult> StartTest();
    public string ToString();
}

public class TestResult
{
    public bool Success { get; }
    public string Message { get; }
    public string Details { get; }

    public TestResult(bool success, string message, string? details = "")
    {
        Success = success;
        Message = message;
		Details = details ?? "";
    }

}
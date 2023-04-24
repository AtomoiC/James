namespace James.Core;

public interface IStartupService
{
    void OnStartup();
}

public interface IAsyncStartupService
{
    Task OnStartupAsync();
}

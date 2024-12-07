namespace SPA.Core.Interfaces;

public interface IResultLogger
{
    void Log(string message);
    void Throw(string message);
}
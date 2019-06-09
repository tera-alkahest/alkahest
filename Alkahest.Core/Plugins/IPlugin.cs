namespace Alkahest.Core.Plugins
{
    public interface IPlugin
    {
        string Name { get; }

        void Start();

        void Stop();
    }
}

namespace Alkahest.Extractor
{
    interface ICommand
    {
        string Name { get; }

        string Syntax { get; }

        string Description { get; }

        int RequiredArguments { get; }

        void Run(string output, string[] args);
    }
}

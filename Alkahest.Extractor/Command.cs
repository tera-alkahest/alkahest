namespace Alkahest.Extractor
{
    abstract class Command
    {
        public abstract string Name { get; }

        public abstract string Syntax { get; }

        public abstract string Description { get; }

        public abstract int RequiredArguments { get; }

        public abstract void Run(string output, string[] args);
    }
}

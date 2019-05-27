namespace Alkahest.Scanner
{
    interface IScanner
    {
        void Run(MemoryReader reader, IpcChannel channel);
    }
}

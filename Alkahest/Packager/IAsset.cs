using System.IO;

namespace Alkahest.Packager
{
    interface IAsset
    {
        FileInfo File { get; }

        bool CheckIfLatest();

        void Update();
    }
}

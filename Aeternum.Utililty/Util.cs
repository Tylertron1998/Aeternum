using System.IO;

namespace Aeternum.Utililty
{
    public static class Util
    {
        public static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path)) return;

            Directory.CreateDirectory(path);
        }
    }
}
namespace JarHell.Packages
{
    public class PackageMeta
    {
        public string Path { get; }

        public PackageInfo PackageInfo { get; }

        public PackageMeta(string path, PackageInfo packageInfo)
        {
            Path = path;
            PackageInfo = packageInfo;
        }
    }
}
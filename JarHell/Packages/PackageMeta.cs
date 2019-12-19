namespace JarHell.Packages
{
    public class PackageMeta
    {
        public string Path { get; }

        public bool Local { get; }

        public PackageInfo PackageInfo { get; }

        public PackageMeta(string path, bool local, PackageInfo packageInfo)
        {
            Local = local;
            Path = path;
            PackageInfo = packageInfo;
        }
    }
}
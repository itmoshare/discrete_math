namespace JarHell.Versions
{
    public static class VersionHelpers
    {
        public static bool IsMatch(this VersionFilter versionFilter, Version version)
        {
            return versionFilter.GetMinVersion() < version && version < versionFilter.GetMaxVersion();
        }
    }
}
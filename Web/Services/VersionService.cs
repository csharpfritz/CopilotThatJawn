using System.Reflection;

namespace Web.Services;

public class VersionService : IVersionService
{
    private readonly string _version;

    public VersionService()
    {
        // Get version from assembly or use build timestamp
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyVersion = assembly.GetName().Version?.ToString() ?? "1.0.0.0";
        
        // Alternative: Use assembly's last write time as version
        var buildDate = File.GetLastWriteTime(assembly.Location);
        _version = buildDate.Ticks.ToString();
        
        // You could also use assembly version: _version = assemblyVersion;
    }

    public string GetVersion()
    {
        return _version;
    }
}

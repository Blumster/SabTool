namespace SabTool.Utils.Misc;

public sealed class Singleton<T> where T : new()
{
    private static readonly Lazy<T> LazyInstance = new(() => new T());

    private Singleton()
    { }

    public static T Instance
        => LazyInstance.Value;
}

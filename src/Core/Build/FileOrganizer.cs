namespace Nabla.TypeScript.Tool;

public abstract class FileOrganizer
{

    public FileOrganizer(IFileNameResolver nameResolver)
    {
        NameResolver = nameResolver;
    }

    public string? DefaultFileName { get; set; }

    public virtual bool ProduceSingleFile => false;

    public IFileNameResolver NameResolver { get; }

    protected string ResolveFileName(object source) => NameResolver.ResolveFileName(source);

    public abstract string ResolveFileName(object source, string? desiredName);

}

public class SingleFileResolver : FileOrganizer
{
    string? _moduleName;

    public SingleFileResolver(IFileNameResolver nameResolver) : base(nameResolver)
    {
    }

    public override bool ProduceSingleFile => true;

    public override string ResolveFileName(object source, string? desiredName)
    {
        if (_moduleName == null)
        {
            _moduleName = desiredName ?? DefaultFileName ?? ResolveFileName(source);
        }

        return _moduleName;
    }

}

public class ExplicitFileResolver : NatureFileResolver
{
    private string? _moduleName;

    public ExplicitFileResolver(IFileNameResolver nameResolver) : base(nameResolver)
    {
    }

    protected override string ResolveUnspecified(object source)
    {
        if (_moduleName == null)
            _moduleName = DefaultFileName ?? ResolveFileName(source);

        return _moduleName;
    }
}

public class NatureFileResolver : FileOrganizer
{
    public NatureFileResolver(IFileNameResolver nameResolver) : base(nameResolver)
    {
    }

    public override string ResolveFileName(object source, string? desiredName)
    {
        if (desiredName != null)
            return desiredName;

        return ResolveUnspecified(source);
    }

    protected virtual string ResolveUnspecified(object source)
    {
        return ResolveFileName(source);
    }
}
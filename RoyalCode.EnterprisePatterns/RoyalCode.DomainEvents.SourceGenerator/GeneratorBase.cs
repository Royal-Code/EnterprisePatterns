using System.Text;

namespace RoyalCode.DomainEvents.SourceGenerator;

public abstract class GeneratorBase
{
    private UsingsGenerator? usingsGenerator;
    private List<Action<StringBuilder>>? bodyGenerators;

    public GeneratorBase(string @namespace, string className)
    {
        Namespace = @namespace;
        ClassName = className;
    }

    public UsingsGenerator UsingsGenerator => usingsGenerator ??= new();

    public string Namespace { get; }

    public string ClassName { get; }

    public void AddBodyGenerator(Action<StringBuilder> generator)
    {
        bodyGenerators ??= new();
        bodyGenerators.Add(generator);
    }

    public void Write(StringBuilder sb)
    {
        if (Namespace is null)
            throw new InvalidOperationException("Namespace is null.");

        if (ClassName is null)
            throw new InvalidOperationException("ClassName is null.");

        if (usingsGenerator is not null)
        {
            usingsGenerator.Write(sb);
            sb.AppendLine();
        }

        sb.AppendLine($"namespace {Namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    public partial class {ClassName}");
        sb.AppendLine("    {");

        if (bodyGenerators is not null)
        {
            foreach (var generator in bodyGenerators)
            {
                generator(sb);
                sb.AppendLine();
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
    }
}


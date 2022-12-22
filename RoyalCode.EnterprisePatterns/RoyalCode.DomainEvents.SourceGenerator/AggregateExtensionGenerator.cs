using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace RoyalCode.DomainEvents.SourceGenerator;

public class AggregateExtensionGenerator : GeneratorBase
{
    public AggregateExtensionGenerator(
        Compilation compilation,
        SourceProductionContext context,
        string className, string classNamespace)
        : base(classNamespace, className)
    {
        Compilation = compilation;
        Context = context;
    }

    public Compilation Compilation { get; }
    
    public SourceProductionContext Context { get; }
    
    public void Generate()
    {
        StringBuilder builder = new();
        Write(builder);
        var source = builder.ToString();

        // write the source
        Context.AddSource($"{ClassName}.g.cs", source);
    }

    public void AddApplyMethod(MethodDeclarationSyntax methodDeclarationSyntax)
    {
        // check if method has only one parameter
        if (methodDeclarationSyntax.ParameterList.Parameters.Count != 1)
        {
            // create diagnostic for the method does not have only one parameter
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "DomainEventsGenerator002",
                    "Method does not have only one parameter",
                    "Method {0} does not have only one parameter",
                    "DomainEventsGenerator",
                    DiagnosticSeverity.Error,
                    true),
                methodDeclarationSyntax.GetLocation(),
                methodDeclarationSyntax.Identifier.Text);

            // add diagnostic
            Context.ReportDiagnostic(diagnostic);

            return;
        }

        // get method first parameter type and name
        var parameter = methodDeclarationSyntax.ParameterList.Parameters[0]!;
        var parameterType = parameter.Type;
        var parameterName = parameter.Identifier;
        
        // check parameterType is not null
        if (parameterType is null)
        {
            // create diagnostic for the method parameter type is undefined
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "DomainEventsGenerator003",
                    "Method parameter type is undefined",
                    "Method {0} parameter type is undefined",
                    "DomainEventsGenerator",
                    DiagnosticSeverity.Error,
                    true),
                methodDeclarationSyntax.GetLocation(),
                methodDeclarationSyntax.Identifier.Text);

            // add diagnostic
            Context.ReportDiagnostic(diagnostic);

            return;
        }

        // get parameter type model
        var parameterTypeModel = Compilation.GetSemanticModel(parameterType.SyntaxTree);
        // get parameter type symbol
        var parameterTypeSymbol = parameterTypeModel.GetSymbolInfo(parameterType).Symbol;
        // try get the parameter type name with containing types
        var parameterTypeName = parameterTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            ?? parameterType.ToString();

        var notNullWhen = TryGetMemberNotNullAttribute(methodDeclarationSyntax);
        if (notNullWhen is not null)
            UsingsGenerator.AddUsing("System.Diagnostics.CodeAnalysis");
        
        AddBodyGenerator(builder =>
        {
            // generate protected Appy method for the event, that AddEvent the event and call the the When.
            if (notNullWhen is not null)
                builder.AppendLine($"        {notNullWhen}");
            builder.AppendLine($"        protected void Apply({parameterTypeName} {parameterName})");
            builder.AppendLine("        {");
            builder.AppendLine($"            AddEvent({parameterName});");
            builder.AppendLine($"            {methodDeclarationSyntax.Identifier.Text}({parameterName});");
            builder.AppendLine("        }");
        });
    }

    private static string? TryGetMemberNotNullAttribute(MethodDeclarationSyntax methodDeclaration)
    {
        var attr = methodDeclaration.TryGetAttribute("MemberNotNull");

        if (attr is null)
            return null;

        return $"[{attr}]";
    }
}


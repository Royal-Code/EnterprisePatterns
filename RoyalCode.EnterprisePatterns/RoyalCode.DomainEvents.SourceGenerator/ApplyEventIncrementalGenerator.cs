using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace RoyalCode.DomainEvents.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ApplyEventIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput((i) => i.AddSource("WhenAttribute.g.cs", AttributeGenerator.AttributeText));

        var methods = context.SyntaxProvider.CreateSyntaxProvider(HasWhenAttribute, GetMethodDeclarationSyntax)
            .Where(static m => m is not null)
            .Collect();

        var compilation = context.CompilationProvider.Combine(methods);

        context.RegisterSourceOutput(compilation,
                static (spc, source) => Execute(source.Item1, source.Item2!, spc));
    }
    
    private static bool HasWhenAttribute(SyntaxNode syntaxNode, CancellationToken _)
        => syntaxNode is MethodDeclarationSyntax method && method.AttributeLists.Count > 0;
    
    public static MethodDeclarationSyntax? GetMethodDeclarationSyntax(
        GeneratorSyntaxContext context,
        CancellationToken token)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        if (methodDeclaration.TryGetAttribute("When") is not null)
            return methodDeclaration;
        return null;
    }

    private static void Execute(Compilation compilation,
        ImmutableArray<MethodDeclarationSyntax> methods,
        SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        // group the fields by class
        var groups = methods.GroupBy(m => m.Parent);

        foreach (var group in groups)
        {
            var classDeclarationSyntax = (group.Key as ClassDeclarationSyntax)!;
            var classMethods = group.ToList();

            if (!GuardParentIsAPartialClass(context, classDeclarationSyntax, classMethods[0].GetLocation()))
                continue;
            
            // get the class model
            var classModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree)
                .GetDeclaredSymbol(classDeclarationSyntax)!;
            // get class name
            var className = classModel.Name;
            // get class namespace
            var classNamespace = classModel.ContainingNamespace.ToDisplayString();

            var generator = new AggregateExtensionGenerator(compilation, context, className!, classNamespace);

            for (int i = 0; i < classMethods.Count; i++)
            {
                generator.AddApplyMethod(methods[i]);
            }

            generator.Generate();
        }
    }

    private static bool GuardParentIsAPartialClass(
        SourceProductionContext context,
        ClassDeclarationSyntax? classDeclaration,
        Location location)
    {
        if (classDeclaration is null || !classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            // create diagnostic for class is not a partial class
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "DomainEventsGenerator001",
                    "Class is not a partial class",
                    "Class {0} is not a partial class",
                    "DomainEventsGenerator",
                    DiagnosticSeverity.Error,
                    true),
                location,
                classDeclaration?.Identifier.Text ?? "Unknown");

            // add diagnostic
            context.ReportDiagnostic(diagnostic);

            return false;
        }

        return true;
    }
}


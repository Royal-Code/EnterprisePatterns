using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace RoyalCode.DomainEvents.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ApplyEventIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput((i) => i.AddSource("WhenAttribute.g.cs", AttributeGenerator.AttributeText));

        var methods = context.SyntaxProvider.CreateSyntaxProvider(HasWhenAttribute, GetMethodDeclarationSyntax)
            .Collect()
            .SelectMany((i, t) => i)
            .Select((i, t) => i);


    }

    private static bool HasWhenAttribute(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var methodDeclaration = (MethodDeclarationSyntax)syntaxNode;
        var attributes = methodDeclaration.AttributeLists;
        foreach (var attribute in attributes)
        {
            var name = ExtractName(attribute.Attributes[0].Name);
            
            if (name == "When" || name == "WhenAttribute")
                return true;
        }
        return false;
    }

    private static string? ExtractName(NameSyntax? name)
   {
      return name switch
      {
         SimpleNameSyntax ins => ins.Identifier.Text,
         QualifiedNameSyntax qns => qns.Right.Identifier.Text,
         _ => null
      };
   }

    public static MethodDeclarationSyntax GetMethodDeclarationSyntax(GeneratorSyntaxContext context, CancellationToken token)
    {
        return (MethodDeclarationSyntax)context.Node;
    }
}


[Generator]
public class ApplyEventGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterForPostInitialization((i) => i.AddSource("WhenAttribute.g.cs", AttributeGenerator.AttributeText));

        context.RegisterForSyntaxNotifications(() => new ApplyEventSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // log to build output
        context.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                "DomainEventsGenerator901",
                "Using Apply Event Generator",
                "Using Apply Event Generator",
                "DomainEventsGenerator",
                DiagnosticSeverity.Info,
                true),
            Location.None));


        // retrieve the populated receiver 
        ApplyEventSyntaxReceiver receiver = (ApplyEventSyntaxReceiver)context.SyntaxContextReceiver!;
        
        // generate a diagnostic
        context.ReportDiagnostic(Diagnostic.Create(
            new DiagnosticDescriptor(
                "DomainEventsGenerator902",
                "Apply Event Syntax Receiver",
                "Apply Event Generator retrieved.",
                "DomainEventsGenerator",
                DiagnosticSeverity.Info,
                true),
            Location.None));

        // group the fields by class
        var groups = receiver.MethodsToProcess.GroupBy(m => m.Parent);

        foreach (var group in groups)
        {
            var classDeclarationSyntax = group.Key as ClassDeclarationSyntax;
            var methods = group.ToList();
            if (GuardParentIsAPartialClass(context, classDeclarationSyntax, methods[0].GetLocation()))
                continue;

            var generator = new AggregateExtensionGenerator(context, classDeclarationSyntax!);
            generator.Open();

            for (int i = 0; i < methods.Count; i++)
            {
                generator.AddApplyMethod(methods[i]);
            }

            generator.Close();
            generator.Generate();
        }
    }

    private bool GuardParentIsAPartialClass(
        GeneratorExecutionContext context,
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

    //private void ProcessMethod()
}

public class AggregateExtensionGenerator
{
    private readonly StringBuilder builder = new();

    public AggregateExtensionGenerator(
        GeneratorExecutionContext context,
        ClassDeclarationSyntax classDeclaration)
    {
        Context = context;
        ClassDeclaration = classDeclaration;
    }

    public GeneratorExecutionContext Context { get; }

    public ClassDeclarationSyntax ClassDeclaration { get; }

    public void Generate()
    {
        // write the source
        Context.AddSource($"{ClassDeclaration.Identifier.Text}.g.cs", builder.ToString());
    }

    public void Open()
    {
        // using: System, System.Collections.Generic, System.Linq, System.Threading.Tasks, RoyalCode.DomainEvents;
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using RoyalCode.DomainEvents;");
        builder.AppendLine();

        // namespace of ClassDeclaration
        builder.AppendLine($"namespace {ClassDeclaration.Parent?.GetFirstToken().Text}");

        // open namespace
        builder.AppendLine("{");

        // open class
        builder.AppendLine($"    public partial class {ClassDeclaration.Identifier.Text}");
        builder.AppendLine("    {");
    }

    public void Close()
    {
        builder.AppendLine("    }");
        builder.AppendLine("}");
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

        // generate private Appy method for the event, that AddEvent the event and call the the When.
        builder.AppendLine($"        protected void Apply({parameterType} {parameterName})");
        builder.AppendLine("        {");
        builder.AppendLine($"            AddEvent({parameterName});");
        builder.AppendLine($"            When({parameterName});");
        builder.AppendLine("        }");
        builder.AppendLine();
    }
}

internal static class AttributeGenerator
{
    internal const string AttributeText = @"
using System;
namespace RoyalCode.DomainEvents
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class WhenAttribute : Attribute
    {
        public WhenAttribute() { }
    }
}
";
}


public class ApplyEventSyntaxReceiver : ISyntaxContextReceiver
{
    public List<MethodDeclarationSyntax> MethodsToProcess { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // get all class methods with the WhenAttribute
        if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax
            && methodDeclarationSyntax.AttributeLists.Count > 0
            && methodDeclarationSyntax.AttributeLists[0].Attributes.Count > 0
            && methodDeclarationSyntax.AttributeLists[0].Attributes[0].Name.ToString() == "WhenAttribute")
        {
            // add diagnostic of the method
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "DomainEventsGenerator903",
                    "Method found",
                    "Method {0} found",
                    "DomainEventsGenerator",
                    DiagnosticSeverity.Info,
                    true),
                methodDeclarationSyntax.GetLocation(),
                methodDeclarationSyntax.Identifier.Text);

            MethodsToProcess.Add(methodDeclarationSyntax);
        }


        // // any method with at least one attribute is a candidate for apply generation
        // if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax
        //     && methodDeclarationSyntax.AttributeLists.Count > 0)
        // {
        //     // get the symbol of the method
        //     var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

        //     // check if method symbos has "RoyalCode.DomainEvents.WhenAttribute"
        //     var whenAttribute = methodSymbol?.GetAttributes()
        //         .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == "RoyalCode.DomainEvents.WhenAttribute");

        //     // check if has whenAttribute
        //     if (whenAttribute is null)
        //         return;

        //     // create and add method to be processed
        //     MethodsToProcess.Add(methodDeclarationSyntax);
        // }

    }
}


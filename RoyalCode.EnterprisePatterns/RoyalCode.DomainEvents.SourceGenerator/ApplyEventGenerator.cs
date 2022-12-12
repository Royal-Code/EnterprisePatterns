using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace RoyalCode.DomainEvents.SourceGenerator;


//[Generator]
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

            //var generator = new AggregateExtensionGenerator(context, classDeclarationSyntax!);
            //generator.Open();

            //for (int i = 0; i < methods.Count; i++)
            //{
            //    generator.AddApplyMethod(methods[i]);
            //}

            //generator.Close();
            //generator.Generate();
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


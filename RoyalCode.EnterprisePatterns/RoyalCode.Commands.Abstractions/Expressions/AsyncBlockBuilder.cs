
using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.Commands.Abstractions.Expressions;

/// <summary>
/// Classe para criar System.Linq.Expression de blocos assíncronos, 
/// onde existem task que precisam ser completadas outros comandos possam ser executados.
/// </summary>
public class AsyncExpressionBlockBuilder
{
    private readonly List<Expression> commands = new();
    private readonly List<ParameterExpression> variables = new();
    private AsyncChainedBlock? current;

    public void AddVariable(ParameterExpression variable)
    {
        variables.Add(variable);
    }

    public void AddCommand(Expression expression)
    {
        if (current is null)
            commands.Add(expression);
        else
            current.Add(expression);
    }

    public void AwaitVoid(Expression taskExpression)
    {
        if (current is null)
            current = new AsyncChainedBlock(taskExpression);
        else
            current = current.Await(taskExpression);
    }

    public Expression AwaitResult(Expression taskExpression, Type resultType) 
    {
        if (current is null)
            current = new AsyncChainedBlock(taskExpression, resultType);
        else
            current = current.Await(taskExpression, resultType);

        return current.AwaitedResultExpression!;
    }

    public Expression Build()
    {
        var taskExpression = current?.CreateTask() ?? Expression.Constant(Task.CompletedTask);

        commands.Add(taskExpression);
        return Expression.Block(typeof(Task), variables, commands);
    }

    public Expression Build(Expression resultExpression, Type resultType)
    {
        var taskExpression = current?.CreateTask(resultExpression, resultType)
            ?? Expression.Call(
                typeof(Task).GetMethods().First(m => m.Name == "FromResult").MakeGenericMethod(resultType),
                resultExpression);

        commands.Add(taskExpression);
        return Expression.Block(typeof(Task<>).MakeGenericType(resultType), variables, commands);
    }
}

internal class AsyncChainedBlock
{
    private readonly Expression taskExpression;
    private readonly Type? taskResultType;
    private readonly AsyncChainedBlock? parent;
    private readonly ParameterExpression continuationParameter;

    private readonly List<Expression> expressions = new();

    public AsyncChainedBlock(Expression taskExpression, Type? taskResultType = null, AsyncChainedBlock? parent = null)
    {
        this.taskExpression = taskExpression;
        this.taskResultType = taskResultType;
        this.parent = parent;

        if (taskResultType is null)
        {
            continuationParameter = Expression.Parameter(typeof(Task), "t");
        }
        else
        {
            continuationParameter = Expression.Parameter(typeof(Task<>).MakeGenericType(taskResultType), "t");
            AwaitedResultExpression = Expression.Property(continuationParameter, "Result");
        }
    }

    public Expression? AwaitedResultExpression { get; }

    public Type? ReturnType { get; set; }

    public void Add(Expression expression)
    {
        expressions.Add(expression);
    }

    public AsyncChainedBlock Await(Expression taskExpression, Type? taskResultType = null)
    {
        if (taskResultType is null)
            ReturnType = typeof(Task);
        else
            ReturnType = typeof(Task<>).MakeGenericType(taskResultType);

        Add(taskExpression);

        return new AsyncChainedBlock(taskExpression, taskResultType, this);
    }

    public Expression CreateTask()
    {
        var task = GetTaskExpressionForBuild();

        var continuationParameterType = ReturnType is null
            ? typeof(Action<>).MakeGenericType(continuationParameter.Type)
            : typeof(Func<,>).MakeGenericType(continuationParameter.Type, ReturnType);

        var continuationMethod = ReturnType is null
                ? task.Type.GetMethod("ContinueWith", new Type[] { continuationParameterType })
                : task.Type.GetMethods()
                    .Where(m => m.Name == "ContinueWith" && m.GetParameters().Length == 1 && m.GetGenericArguments().Length == 1)
                    .Select(m => m.MakeGenericMethod(ReturnType))
                    .First(m => m.GetParameters()[0].ParameterType == continuationParameterType);
        
        var continuationBlock = ReturnType is null
            ? Expression.Block(expressions)
            : Expression.Block(ReturnType, expressions);

        var continuationLambda = Expression.Lambda(continuationParameterType, continuationBlock, continuationParameter);

        return Expression.Call(task, continuationMethod!, continuationLambda);
    }

    public Expression CreateTask(Expression resultExpression, Type resultType)
    {
        Add(resultExpression);
        ReturnType = resultType;
        return CreateTask();
    }

    private Expression GetTaskExpressionForBuild()
    {
        if (parent is null)
        {
            return taskExpression;
        }
        else
        {
            var parentTask = parent.CreateTask();

            var unwrapMethod = typeof(TaskExtensions)
                .GetMethods()
                .First(m => m.Name == "Unwrap" && m.GetGenericArguments().Length == (taskResultType is null ? 0 : 1));

            if (taskResultType is not null)
                unwrapMethod = unwrapMethod.MakeGenericMethod(taskResultType);

            return Expression.Call(unwrapMethod!, parentTask);
        }
    }
}
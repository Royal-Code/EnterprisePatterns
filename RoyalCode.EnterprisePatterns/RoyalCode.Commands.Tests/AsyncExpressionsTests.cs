
using RoyalCode.Commands.Abstractions.Expressions;
using System.Linq.Expressions;

namespace RoyalCode.Commands.Tests;

public class AsyncExpressionsTests
{
    [Fact]
    public async Task ManualAwait()
    {
        var repo = new Repo();
        var text = await repo.GetAsync();
        Assert.Equal("Hello World!", text);
    }

    [Fact]
    public void ManualTaskContinuation()
    {
        var repo = new Repo();
        string? text = null;

        var task = repo.GetAsync()
            .ContinueWith(t =>
            {
                text = t.Result;
            });

        var task2 = repo.GetAsync()
            .ContinueWith(t =>
            {
                text = t.Result;
                return text;
            });

        task.Wait();
        Assert.Equal("Hello World!", text);
    }

    [Fact]
    public void ManualTaskContinuationExtended()
    {
        var repo = new Repo();
        string? text1 = null;
        string? text2 = null;
        string? text3 = null;

        var task = repo.GetAsync()
            .ContinueWith(t =>
            {
                text1 = t.Result;
                return repo.GetAsync();
            })
            .Unwrap().ContinueWith(t =>
            {
                text2 = t.Result;
                return repo.GetAsync();
            })
            .Unwrap().ContinueWith(t =>
            {
                text3 = t.Result;
                return new Values()
                {
                    First = text1,
                    Second = text2,
                    Third = text3
                };
            });

        var result = task.Result;
        Assert.Equal("Hello World!", result.First);
        Assert.Equal("Hello World!", result.Second);
        Assert.Equal("Hello World!", result.Third);
    }

    [Fact]
    public void Task_WithoutResult_NoContinuation()
    {
        var asyncBuilder = new AsyncExpressionBlockBuilder();

        var textVar = Expression.Variable(typeof(string), "text");
        var assign = Expression.Assign(textVar, Expression.Constant("Hello", typeof(string)));
        
        asyncBuilder.AddVariable(textVar);
        asyncBuilder.AddCommand(assign);

        var taskBlock = asyncBuilder.Build();

        var lambda = Expression.Lambda<Func<Task>>(taskBlock);
        var func = lambda.Compile();
        var task = func();
        task.GetAwaiter().GetResult();
    }

    [Fact]
    public void Task_Result_NoContinuation()
    {
        var asyncBuilder = new AsyncExpressionBlockBuilder();

        var textVar = Expression.Variable(typeof(string), "text");
        var assign = Expression.Assign(textVar, Expression.Constant("Hello", typeof(string)));

        asyncBuilder.AddVariable(textVar);
        asyncBuilder.AddCommand(assign);

        var taskBlock = asyncBuilder.Build(textVar, typeof(string));
        
        var lambda = Expression.Lambda<Func<Task<string>>>(taskBlock);
        var func = lambda.Compile();
        var task = func();
        var text = task.GetAwaiter().GetResult();
        Assert.Equal("Hello", text);
    }

    [Fact]
    public void Task_WithoutResult_OneContinuation()
    {
        var asyncBuilder = new AsyncExpressionBlockBuilder();

        var repoVar = Expression.Variable(typeof(Repo), "repo");
        var assignRepo = Expression.Assign(repoVar, Expression.New(typeof(Repo)));
        asyncBuilder.AddVariable(repoVar);
        asyncBuilder.AddCommand(assignRepo);

        var textVar = Expression.Variable(typeof(string), "text");
        asyncBuilder.AddVariable(textVar);

        var getAsyncMethod = typeof(Repo).GetMethod(nameof(Repo.GetAsync))!;
        var getAsyncCall = Expression.Call(repoVar, getAsyncMethod);
        var getResult = asyncBuilder.AwaitResult(getAsyncCall, typeof(string));

        var assignText = Expression.Assign(textVar, getResult);
        asyncBuilder.AddCommand(assignText);

        var taskBlock = asyncBuilder.Build();

        var lambda = Expression.Lambda<Func<Task>>(taskBlock);
        var func = lambda.Compile();
        var task = func();
        task.GetAwaiter().GetResult();
    }

    [Fact]
    public void Task_Result_OneContinuation()
    {
        var asyncBuilder = new AsyncExpressionBlockBuilder();

        var repoVar = Expression.Variable(typeof(Repo), "repo");
        var assignRepo = Expression.Assign(repoVar, Expression.New(typeof(Repo)));
        asyncBuilder.AddVariable(repoVar);
        asyncBuilder.AddCommand(assignRepo);

        var textVar = Expression.Variable(typeof(string), "text");
        asyncBuilder.AddVariable(textVar);

        var getAsyncMethod = typeof(Repo).GetMethod(nameof(Repo.GetAsync))!;
        var getAsyncCall = Expression.Call(repoVar, getAsyncMethod);
        var getResult = asyncBuilder.AwaitResult(getAsyncCall, typeof(string));

        var assignText = Expression.Assign(textVar, getResult);
        asyncBuilder.AddCommand(assignText);

        var taskBlock = asyncBuilder.Build(textVar, typeof(string));

        var lambda = Expression.Lambda<Func<Task<string>>>(taskBlock);
        var func = lambda.Compile();
        var task = func();
        var text = task.GetAwaiter().GetResult();
        Assert.Equal("Hello World!", text);
    }

    [Fact]
    public void Task_Result_ThreeContinuation()
    {
        var asyncBuilder = new AsyncExpressionBlockBuilder();

        var repoVar = Expression.Variable(typeof(Repo), "repo");
        var assignRepo = Expression.Assign(repoVar, Expression.New(typeof(Repo)));
        asyncBuilder.AddVariable(repoVar);
        asyncBuilder.AddCommand(assignRepo);

        var textVar1 = Expression.Variable(typeof(string), "text1");
        var textVar2 = Expression.Variable(typeof(string), "text2");
        var textVar3 = Expression.Variable(typeof(string), "text3");
        asyncBuilder.AddVariable(textVar1);
        asyncBuilder.AddVariable(textVar2);
        asyncBuilder.AddVariable(textVar3);

        var getAsyncMethod = typeof(Repo).GetMethod(nameof(Repo.GetAsync))!;

        var getAsyncCall = Expression.Call(repoVar, getAsyncMethod);
        var getResult = asyncBuilder.AwaitResult(getAsyncCall, typeof(string));
        var assignText1 = Expression.Assign(textVar1, getResult);
        asyncBuilder.AddCommand(assignText1);

        getAsyncCall = Expression.Call(repoVar, getAsyncMethod);
        getResult = asyncBuilder.AwaitResult(getAsyncCall, typeof(string));
        var assignText2 = Expression.Assign(textVar2, getResult);
        asyncBuilder.AddCommand(assignText2);

        getAsyncCall = Expression.Call(repoVar, getAsyncMethod);
        getResult = asyncBuilder.AwaitResult(getAsyncCall, typeof(string));
        var assignText3 = Expression.Assign(textVar3, getResult);
        asyncBuilder.AddCommand(assignText3);

        var valuesVar = Expression.Variable(typeof(Values), "values");
        var valuesCtor = typeof(Values).GetConstructor(new[] { typeof(string), typeof(string), typeof(string) })!;
        var valuesNew = Expression.New(valuesCtor, textVar1, textVar2, textVar3);
        var valuesAssign = Expression.Assign(valuesVar, valuesNew);
        asyncBuilder.AddVariable(valuesVar);
        asyncBuilder.AddCommand(valuesAssign);

        var taskBlock = asyncBuilder.Build(valuesVar, typeof(Values));

        var lambda = Expression.Lambda<Func<Task<Values>>>(taskBlock);
        var func = lambda.Compile();
        var task = func();
        var values = task.GetAwaiter().GetResult();
        Assert.Equal("Hello World!", values.First);
        Assert.Equal("Hello World!", values.Second);
        Assert.Equal("Hello World!", values.Third);
    }
}

public class Repo
{
    public async Task<string> GetAsync()
    {
        await Task.Delay(1000);
        return "Hello World!";
    }
}

public class Values
{
    public Values() { }

    public Values(string first, string second, string third)
    {
        First = first;
        Second = second;
        Third = third;
    }

    public string First { get; set; }

    public string Second { get; set; }

    public string Third { get; set; }
}
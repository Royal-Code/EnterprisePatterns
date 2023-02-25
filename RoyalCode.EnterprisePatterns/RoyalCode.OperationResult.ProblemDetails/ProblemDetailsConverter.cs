namespace RoyalCode.OperationResult.ProblemDetails;

/// <summary>
/// Converts the <see cref="IOperationResult"/> to <see cref="ProblemDetails"/>.
/// </summary>
public static class ProblemDetailsConverter
{

    public static Microsoft.AspNetCore.Mvc.ProblemDetails ToProblemDetails(
       this IOperationResult result, ProblemDetailsOptions options)
    {
        if (result.Success)
            return null!;




        // get the description
        //var description = options.Descriptor
    }

    public static ResultProblemCode GetResultProblemCode(this IOperationResult result)
    {
        var code = result.Code;
        var aggregate = false;

        if (code is null)
        {
            code = result.GetType().Name;
            aggregate = true;
        }

        return new ResultProblemCode
        {
            Code = code,
            Aggregate = aggregate
        };
    }
}

public struct ResultProblemCode
{
    public string Code { get; set; }

    public bool Aggregate { get; set; }
}
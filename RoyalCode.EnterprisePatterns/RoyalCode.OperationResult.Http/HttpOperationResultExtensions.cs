using RoyalCode.OperationResult;
using RoyalCode.OperationResult.Serialization;
using System.Net.Http.Json;

namespace System.Net.Http;

/// <summary>
/// <para>
///     Extension methods for deserialize <see cref="IOperationResult" /> from <see cref="HttpResponseMessage"/>.
/// </para>
/// </summary>
public static class HttpOperationResultExtensions
{
    
    public static async Task<IOperationResult> ToOperationResultAsync(
        this HttpResponseMessage response, CancellationToken token = default)
    {

        if (response.IsSuccessStatusCode)
        {
            // on success, when there is no value,
            // returns a success OperationResult with no message
            return BaseResult.ImmutableSuccess;
        }
        else
        {
            // in case of errors, the OperationResult must be deserialised
            var result = await response.Content.ReadFromJsonAsync(
                SerializationContext.Default.DeserializableResult,
                token);

            result ??= new DeserializableResult();

            // set Success as false
            result.Success = false;

            if (result.Messages is not null)
            {
                // provides the status code of the response for each message
                var status = (int)response.StatusCode;
                foreach (var message in result.Messages)
                {
                    message.Status = (HttpStatusCode)status;
                }
            }

            // returns the new OperationResult
            return result;
        }
    }

    public static async Task<IOperationResult<TValue>> ToOperationResultAsync<TValue>(
        this HttpResponseMessage response, CancellationToken token = default)
    {
        if (response.IsSuccessStatusCode)
        {
            // on success, with value, the value must be deserialized
            var value = await response.Content
                .ReadFromJsonAsync<TValue>();

            return ValueResult.Create(value!);
        }
        else
        {
            // in case of errors, the OperationResult must be deserialised
            var result = await response.Content.ReadFromJsonAsync<DeserializableResult<TValue>>(
                cancellationToken: token);

            result ??= new DeserializableResult<TValue>();

            // set Success as false
            result.Success = false;

            if (result.Messages is not null)
            {
                // provides the status code of the response for each message
                var status = (int)response.StatusCode;
                foreach (var message in result.Messages)
                {
                    message.Status = (HttpStatusCode)status;
                }
            }

            // retorna o OperationResult
            return result;
        }
    }


    //    /// <summary>
    //    /// <para>
    //    ///     Get <see cref="IOperationResult" /> from <see cref="HttpResponseMessage"/>.
    //    /// </para>
    //    /// </summary>
    //    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    //    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    //    /// <returns>The <see cref="IOperationResult"/>.</returns>
    //    public static async Task<IOperationResult> GetOperationResultAsync(
    //        this HttpResponseMessage response,
    //        CancellationToken cancellationToken = default)
    //    {
    //        // check that the content is json
    //        if (response.Content.Headers.ContentType?.MediaType == "application/json")
    //        {
    //            // try to obtain the result of the operation
    //            var operationResult = await response.Content.ReadFromJsonAsync(
    //                ResultsSerializeContext.Default.DeserializableResult,
    //                cancellationToken);

    //            if (operationResult is not null)
    //                return operationResult;
    //        }

    //        // if is not json, reads the content as string
    //#if NET5_0_OR_GREATER
    //        var text = await response.Content.ReadAsStringAsync(cancellationToken);
    //#else
    //        var text = await response.Content.ReadAsStringAsync();
    //#endif

    //        // if the status code is success, returns a success result.
    //        if (response.IsSuccessStatusCode)
    //        {
    //            // if it is not empty, returns a success message
    //            return string.IsNullOrWhiteSpace(text)
    //                ? BaseResult.ImmutableSuccess
    //                : BaseResult.Create().WithSuccess(text);
    //        }

    //        // if it is not successful, then generates an error result.
    //        switch (response.StatusCode)
    //        {
    //            case HttpStatusCode.BadRequest:
    //                return BaseResult.ValidationError(text);
    //            case HttpStatusCode.Forbidden:
    //                return BaseResult.Forbidden(text);
    //            case HttpStatusCode.NotFound:
    //                return BaseResult.NotFound(text);
    //            case HttpStatusCode.InternalServerError:
    //                return BaseResult.Failure(text, code: GenericErrorCodes.ApplicationError, httpStatus: HttpStatusCode.InternalServerError);

    //            // case of redirection, generates the result with the location
    //            case HttpStatusCode.Moved:
    //            case HttpStatusCode.Redirect:
    //            case HttpStatusCode.RedirectKeepVerb:
    //            case HttpStatusCode.RedirectMethod:
    //                return BaseResult.Failure(text).WithInfo($"Location: {response.Headers.Location}");

    //            default:
    //                return BaseResult.Failure(text, code: response.StatusCode.ToString(), httpStatus: response.StatusCode);
    //        }
    //    }

    //    /// <summary>
    //    /// <para>
    //    ///     Get <see cref="IOperationResult{T}" /> from <see cref="HttpResponseMessage"/>.
    //    /// </para>
    //    /// </summary>
    //    /// <typeparam name="TValue">The type of the value.</typeparam>
    //    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    //    /// <param name="isPlanOnSuccess">The flag to plan the value on success.</param>
    //    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    //    /// <returns>The <see cref="IOperationResult{T}"/>.</returns>
    //    public static async Task<IOperationResult<TValue>> GetOperationResultAsync<TValue>(
    //        this HttpResponseMessage response,
    //        bool isPlanOnSuccess = false,
    //        CancellationToken cancellationToken = default)
    //    {
    //        // try to obtain the result of the operation
    //        if (response.Content.Headers.ContentType?.MediaType == "application/json")
    //        {
    //            if (isPlanOnSuccess && response.IsSuccessStatusCode)
    //            {
    //                // if is plain on success, deserializes TValue and generates the success result
    //                var value = await response.Content.ReadFromJsonAsync<TValue>(cancellationToken: cancellationToken);
    //                return ValueResult.Create(value!);
    //            }

    //            // try to obtain the result of the operation
    //            var operationResult = await response.Content.ReadFromJsonAsync<DeserializableResult<TValue>>(cancellationToken: cancellationToken);

    //            if (operationResult != null)
    //                return operationResult;
    //        }

    //        // if is not json, reads the content as string
    //#if NET5_0_OR_GREATER
    //        var text = await response.Content.ReadAsStringAsync(cancellationToken);
    //#else
    //        var text = await response.Content.ReadAsStringAsync();
    //#endif

    //        // if the status code is success, returns a success result.
    //        if (response.IsSuccessStatusCode)
    //        {
    //            TValue value = default!;
    //            var result = ValueResult.Create(value);
    //            if (!string.IsNullOrWhiteSpace(text))
    //                result.WithSuccess(text);

    //            return result;
    //        }

    //        // if it is not successful, then it generates an error result.
    //        switch (response.StatusCode)
    //        {
    //            case HttpStatusCode.BadRequest:
    //                return ValueResult.ValidationError<TValue>(text);
    //            case HttpStatusCode.Forbidden:
    //                return ValueResult.Forbidden<TValue>(text);
    //            case HttpStatusCode.NotFound:
    //                return ValueResult.NotFound<TValue>(text);
    //            case HttpStatusCode.InternalServerError:
    //                return ValueResult.CreateFailure<TValue>(text, 
    //                    code: GenericErrorCodes.ApplicationError,
    //                    httpStatus: HttpStatusCode.InternalServerError);

    //            // case of redirection, generates the result with the location
    //            case HttpStatusCode.Moved:
    //            case HttpStatusCode.Redirect:
    //            case HttpStatusCode.RedirectKeepVerb:
    //            case HttpStatusCode.RedirectMethod:
    //                return ValueResult.CreateFailure<TValue>(text).WithInfo($"Location: {response.Headers.Location}");

    //            default:
    //                return ValueResult.CreateFailure<TValue>(text,
    //                    code: response.StatusCode.ToString(), 
    //                    httpStatus: response.StatusCode);
    //        }
    //    }
}

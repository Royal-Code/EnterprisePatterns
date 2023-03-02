using RoyalCode.OperationResult;
using RoyalCode.OperationResult.Serialization;
using System.Net.Http.Json;
using System.Text.Json;

namespace System.Net.Http;

/// <summary>
/// <para>
///     Extension methods for deserialize <see cref="IOperationResult" /> from <see cref="HttpResponseMessage"/>.
/// </para>
/// </summary>
public static class HttpOperationResultExtensions
{
    /// <summary>
    /// <para>
    ///     Get <see cref="IOperationResult" /> from <see cref="HttpResponseMessage"/>.
    /// </para>
    /// </summary>
    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    /// <param name="token">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IOperationResult"/>.</returns>
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
            // check if the content is not json
            if (response.Content.Headers.ContentType?.MediaType != "application/json")
            {
                return await response.ReadNonJsonContent(token);
            }

            // in case of errors, the OperationResult must be deserialised
            var result = await response.Content.ReadFromJsonAsync(
                DeserializableResult.JsonTypeInfo,
                token) ?? new DeserializableResult();

            result.Messages ??= new List<ResultMessage>();

            if (result.Messages.Count == 0)
            {
                // if there is no message, add a default message
                result.Messages.Add(new ResultMessage(response.ReasonPhrase ?? response.StatusCode.ToString(),
                    null, null, response.StatusCode));
            }
            else
            {
                var status = response.StatusCode;
                // provides the status code of the response for each message
                foreach (var message in result.Messages)
                {
                    message.Status = status;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// <para>
    ///     Get <see cref="IOperationResult{TValue}" /> from <see cref="HttpResponseMessage"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
    /// <param name="options">
    ///     The <see cref="JsonSerializerOptions"/> for the <typeparamref name="TValue"/>, 
    ///     used when status code is success.
    /// </param>
    /// <param name="token">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IOperationResult{TValue}"/>.</returns>
    public static async Task<IOperationResult<TValue>> ToOperationResultAsync<TValue>(
        this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken token = default)
    {
        if (response.IsSuccessStatusCode)
        {
            // on success, with value, the value must be deserialized
            var value = await response.Content.ReadFromJsonAsync<TValue>(options, token);

            return ValueResult.Create(value!);
        }
        else
        {
            // check if the content is not json
            if (response.Content.Headers.ContentType?.MediaType != "application/json")
            {
                return (await response.ReadNonJsonContent(token)).ToValue<TValue>();
            }

            // in case of errors, the OperationResult must be deserialised
            var result = await response.Content.ReadFromJsonAsync<DeserializableResult<TValue>>(
                cancellationToken: token) ?? new DeserializableResult<TValue>();

            result.Messages ??= new List<ResultMessage>();

            if (result.Messages.Count == 0)
            {
                // if there is no message, add a default message
                result.Messages.Add(new ResultMessage(response.ReasonPhrase ?? response.StatusCode.ToString(), 
                    null, null, response.StatusCode));
            }
            else
            {
                var status = response.StatusCode;
                // provides the status code of the response for each message
                foreach (var message in result.Messages)
                {
                    message.Status = status;
                }
            }

            return result;
        }
    }

    private static async Task<BaseResult> ReadNonJsonContent(
        this HttpResponseMessage response, CancellationToken token = default)
    {
        // when is not json, try reads the content as string, if the response has content
        string? text = null;
        if (response.Content.Headers.ContentLength > 0)
        {
#if NET6_0_OR_GREATER
            text = await response.Content.ReadAsStringAsync(token);
#else
            text = await response.Content.ReadAsStringAsync();
#endif
        }

        // create a BaseResult with the status code and the content as message
        var failureResult = BaseResult.Create();
        if (text is not null)
            failureResult.WithError(text, response.StatusCode);
        else
            failureResult.WithError(response.ReasonPhrase ?? response.StatusCode.ToString(), response.StatusCode);

        return failureResult;
    }
}

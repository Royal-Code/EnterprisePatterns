using Microsoft.AspNetCore.Mvc;
using RoyalCode.OperationResults.TestApi.Application.ResultsModels;

namespace RoyalCode.OperationResults.TestApi.Controllers;

[ApiController]
[Route("[controller]/[Action]")]
public class ControllersResultsController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetSimpleValues()
    {
        // cria novo resultado de sucesso a partir do resultado.
        OperationResult<SimpleValues> result = new SimpleValues();

        return result.ToResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetSimpleValuesWithCreatedPath()
    {
        // cria novo resultado de sucesso a partir do resultado.
        OperationResult<SimpleValues> result = new SimpleValues();

        return result.ToResult("/simple-values");
    }

    [HttpGet]
    public async Task<IActionResult> GetSimpleValuesWithError()
    {
        // cria novo resultado de erro a partir do resultado.
        OperationResult<SimpleValues> result = ResultMessage.Error("Erro ao obter valores simples.");

        return result.ToResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetSimpleValuesWithErrorWithCreatedPath()
    {
        // cria novo resultado de erro a partir do resultado.
        OperationResult<SimpleValues> result = ResultMessage.Error("Erro ao obter valores simples.");

        return result.ToResult("/simple-values");
    }

    [HttpGet]
    public async Task<IActionResult> GetSimpleValuesWithCreatedPathAndFormat()
    {
        // cria novo resultado de sucesso a partir do resultado.
        OperationResult<SimpleValues> simpleResult = new SimpleValues();

        var result = simpleResult.Convert(v => v.Number);

        return result.ToResult("/simple-values/{0}", true);
    }

    [HttpGet]
    public async Task<IActionResult> GetSimpleValuesWithErrorWithCreatedPathAndFormat()
    {
        // cria novo resultado de erro a partir do resultado.
        OperationResult<SimpleValues> simpleResult = ResultMessage.Error("Erro ao obter valores simples.");

        var result = simpleResult.Convert(v => v.Number);

        return result.ToResult("/simple-values/{0}", true);
    }

    [HttpGet]
    public async Task<IActionResult> GetSimpleValuesIfValidInput([FromQuery] string? error)
    {
        ValidableResult result = new();

        if (!string.IsNullOrEmpty(error))
        {
            result += ResultMessage.InvalidParameter("Parâmetro inválido.", nameof(error));
        }

        var simpleResult = result.Convert(() => new SimpleValues());

        return simpleResult.ToResult();
    }

    [HttpGet]
    public async Task<IActionResult> ValidableResult([FromQuery] string? input)
    {
        ValidableResult result = new();

        if (string.IsNullOrWhiteSpace(input))
        {
            result += ResultMessage.InvalidParameter("Input inválido.", nameof(input));
        }

        return result.ToResult();
    }
}

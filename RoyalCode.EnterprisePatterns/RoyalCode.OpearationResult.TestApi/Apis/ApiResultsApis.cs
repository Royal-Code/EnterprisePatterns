﻿using Microsoft.AspNetCore.Mvc;
using RoyalCode.OperationResults.TestApi.Application.ResultsModels;

namespace RoyalCode.OperationResults.TestApi.Apis;

public static class ApiResultsApis
{
    public static void MapApiResults(this WebApplication app)
    {
        var group = app.MapGroup("/api/results");

        group.MapGet("GetSimpleValues", GetSimpleValues)
            .WithName("GetSimpleValues")
            .WithTags("GetSimpleValues")
            .WithDescription("Obtém valores simples.")
            .WithOpenApi();

        group.MapGet("GetSimpleValuesWithCreatedPath", GetSimpleValuesWithCreatedPath)
            .WithName("GetSimpleValuesWithCreatedPath")
            .WithDescription("Obtém valores simples com caminho criado.")
            .WithOpenApi();
        
        group.MapGet("GetSimpleValuesWithError", GetSimpleValuesWithError)
            .WithName("GetSimpleValuesWithError")
            .WithDescription("Obtém valores simples com erro.")
            .WithOpenApi();

        group.MapGet("GetSimpleValuesWithErrorWithCreatedPath", GetSimpleValuesWithErrorWithCreatedPath)
            .WithName("GetSimpleValuesWithErrorWithCreatedPath")
            .WithDescription("Obtém valores simples com erro e caminho criado.")
            .WithOpenApi();

        group.MapGet("GetSimpleValuesWithCreatedPathAndFormat", GetSimpleValuesWithCreatedPathAndFormat)
            .WithName("GetSimpleValuesWithCreatedPathAndFormat")
            .WithDescription("Obtém valores simples com caminho criado e formato.")
            .WithOpenApi();

        group.MapGet("GetSimpleValuesWithErrorWithCreatedPathAndFormat", GetSimpleValuesWithErrorWithCreatedPathAndFormat)
            .WithName("GetSimpleValuesWithErrorWithCreatedPathAndFormat")
            .WithDescription("Obtém valores simples com erro, caminho criado e formato.")
            .WithOpenApi();

        group.MapGet("GetSimpleValuesIfValidInput", GetSimpleValuesIfValidInput)
            .WithName("GetSimpleValuesIfValidInput")
            .WithDescription("Obtém valores simples se o input for válido.")
            .WithOpenApi();

        group.MapGet("ValidableResult", ValidableResult)
            .WithName("ValidableResult")
            .WithDescription("Obtém valores simples se o input for válido.")
            .WithOpenApi();
    }

    private static IResult GetSimpleValues()
    {
        // cria novo resultado de sucesso a partir do resultado.
        OperationResult<SimpleValues> result = new SimpleValues();

        return Results.Extensions.ToResult(result);
    }

    private static IResult GetSimpleValuesWithCreatedPath()
    {
        // cria novo resultado de sucesso a partir do resultado.
        OperationResult<SimpleValues> result = new SimpleValues();

        return Results.Extensions.ToResult(result, "/simple-values");
    }

    private static IResult GetSimpleValuesWithError()
    {
        // cria novo resultado de erro a partir do resultado.
        OperationResult<SimpleValues> result = ResultMessage.Error("Erro ao obter valores simples.");

        return Results.Extensions.ToResult(result);
    }

    private static IResult GetSimpleValuesWithErrorWithCreatedPath()
    {
        // cria novo resultado de erro a partir do resultado.
        OperationResult<SimpleValues> result = ResultMessage.Error("Erro ao obter valores simples.");

        return Results.Extensions.ToResult(result, "/simple-values");
    }

    private static IResult GetSimpleValuesWithCreatedPathAndFormat()
    {
        // cria novo resultado de sucesso a partir do resultado.
        OperationResult<SimpleValues> simpleResult = new SimpleValues();

        var result = simpleResult.Convert(v => v.Number);

        return Results.Extensions.ToResult(result, "/simple-values/{0}", true);
    }

    private static IResult GetSimpleValuesWithErrorWithCreatedPathAndFormat()
    {
        // cria novo resultado de erro a partir do resultado.
        OperationResult<SimpleValues> simpleResult = ResultMessage.Error("Erro ao obter valores simples.");

        var result = simpleResult.Convert(v => v.Number);

        return Results.Extensions.ToResult(result, "/simple-values/{0}", true);
    }

    private static IResult GetSimpleValuesIfValidInput([FromQuery] string? error)
    {
        ValidableResult result = new();

        if (!string.IsNullOrEmpty(error))
        {
            result += ResultMessage.InvalidParameter("Parâmetro inválido.", nameof(error));
        }

        var simpleResult = result.Convert(() => new SimpleValues());

        return Results.Extensions.ToResult(simpleResult);
    }

    public static IResult ValidableResult([FromQuery] string? input)
    {
        ValidableResult result = new();

        if (string.IsNullOrWhiteSpace(input))
        {
            result += ResultMessage.InvalidParameter("Input inválido.", nameof(input));
        }

        return Results.Extensions.ToResult(result);
    }
}

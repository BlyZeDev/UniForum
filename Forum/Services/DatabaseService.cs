﻿namespace Forum.Services;

using Dapper;
using FluentResults;
using MySqlConnector;
using System.Collections.Immutable;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public sealed class DatabaseService
{
    private const string CustomSqlState = "45000";
    private static readonly ImmutableArray<MySqlErrorCode> _allowedErrorCodes = [MySqlErrorCode.DuplicateKeyEntry];

    private readonly ILogger<DatabaseService> _logger;
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("Localhost") ?? throw new NullReferenceException("The connection string is missing");
    }

    public Task<Result<User>> GetUserAsync(string email)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@Email", email, DbType.String, ParameterDirection.Input);

        return TryQueryFirstAsync<User>("select * from Users where Email = @Email", dynamicParameters);
    }

    private async Task<Result<T>> TryQueryFirstAsync<T>(string sql, DynamicParameters parameters, CommandType type = CommandType.Text)
    {
        try
        {
            await using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryFirstAsync<T>(sql, parameters, commandType: type);
                return Result.Ok(result);
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (MySqlException ex)
        {
            if (ex.SqlState != CustomSqlState && !_allowedErrorCodes.Contains(ex.ErrorCode))
            {
                _logger.LogCritical(ex, ex.Message);
            }

            return Result.Fail(ex.Message);
        }
    }
}
namespace Forum.Services;

using Dapper;
using FluentResults;
using MySqlConnector;
using System.Collections.Immutable;
using System.Data;

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

    public async Task<Result<bool>> IsEmailTakenAsync(string email)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@Email", email, DbType.String, ParameterDirection.Input);

        var result = await TryQueryFirstOrDefaultAsync<int?>("select 1 from Users where Email = @Email", dynamicParameters);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(result.Value.HasValue);
    }

    public async Task<Result<bool>> IsUsernameTakenAsync(string username)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@Username", username, DbType.String, ParameterDirection.Input);

        var result = await TryQueryFirstOrDefaultAsync<int?>("select 1 from Users where Username = @Username", dynamicParameters);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(result.Value.HasValue);
    }

    public Task<Result<User>> GetUserAsync(string email)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@Email", email, DbType.String, ParameterDirection.Input);

        return TryQueryFirstAsync<User>("select * from Users where Email = @Email", dynamicParameters);
    }

    public async IAsyncEnumerable<EntryDto> EnumerateEntriesAsync()
    {
        await using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            await foreach (var entry in connection.QueryUnbufferedAsync("select e.Author, u.Username, e.CreatedAt, e.Title, e.Text from Entries e join Users u on e.Author = u.Email order by e.CreatedAt desc"))
            {
                yield return new EntryDto
                {
                    Entry = new Entry
                    {
                        Author = entry.Author,
                        CreatedAt = entry.CreatedAt,
                        Title = entry.Title,
                        Text = entry.Text
                    },
                    Username = entry.Username
                };
            }
        }
    }

    public async Task<Result> RegisterUserAsync(string email, string hashedPassword, string username)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@_email", email, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@_password", hashedPassword, DbType.Binary, ParameterDirection.Input);
        dynamicParameters.Add("@_username", username, DbType.String, ParameterDirection.Input);

        var result = await ExecuteAsync("register_user", dynamicParameters, CommandType.StoredProcedure);
        return Result.OkIf(result.ValueOrDefault > 0, "No database entry was written");
    }

    public async Task<Result> CreateEntryAsync(string email, string title, string text)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@_email", email, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@_title", title, DbType.Binary, ParameterDirection.Input);
        dynamicParameters.Add("@_text", text, DbType.String, ParameterDirection.Input);

        var result = await ExecuteAsync("create_entry", dynamicParameters, CommandType.StoredProcedure);
        return Result.OkIf(result.ValueOrDefault > 0, "No database entry was written");
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

    private async Task<Result<T?>> TryQueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters, CommandType type = CommandType.Text)
    {
        try
        {
            await using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: type);
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

    private async Task<Result<int>> ExecuteAsync(string sql, DynamicParameters parameters, CommandType type = CommandType.Text)
    {
        try
        {
            await using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.ExecuteAsync(sql, parameters, commandType: type);
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
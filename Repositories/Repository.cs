using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Fidebill_clientes_back.Repositories;

public class Repository
{
    private readonly string _connectionString;

    public Repository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AzureConnectionString");
    }

    public async Task<int> DeleteAsync(string query)
    {
        using SqlConnection connection = new(_connectionString);
        return await connection.ExecuteAsync(query);
    }

    public async Task<T> GetOneByQuery<T>(string query)
    {
        using SqlConnection connection = new(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<T>(query);
    }

    public async Task<T> GetOneByProcedure<T>(string query, DynamicParameters parameters = null)
    {
        try{
            using SqlConnection connection = new(_connectionString);
            if (parameters == null)
            {
                return await connection.QueryFirstOrDefaultAsync<T>(query, commandType: CommandType.StoredProcedure);
            }
            else
            {
                return await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }catch{
            throw;
        }
    }

    public async Task<List<T>> GetListByProcedure<T>(
        string query,
        DynamicParameters parameters = null
    )
    {
        using SqlConnection connection = new(_connectionString);
        IEnumerable<T> rows;
        if (parameters == null)
        {
            rows = await connection.QueryAsync<T>(query, commandType: CommandType.StoredProcedure);
        }
        else
        {
            rows = await connection.QueryAsync<T>(
                query,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        return rows.AsList();
    }

    public async Task<int> InsertByProcedure(string query, DynamicParameters parameters)
    {
        using SqlConnection connection = new(_connectionString);
        try
        {
            return await connection.ExecuteAsync(
                query,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

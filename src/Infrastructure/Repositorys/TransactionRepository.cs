using Core.Servises.TransactionService;
using Core.Transaction;
using Npgsql;

namespace Infrastructure;

public class TransactionRepository : ITransactionRepository
{
    private readonly string _connectionString;

    public TransactionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Save(TransactionATM account)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string insertCommand = @"
            INSERT INTO transactions (account_id, amount, type)
            VALUES (@account_id, @amount, @type);";

        using var cmd = new NpgsqlCommand(insertCommand, connection);
        cmd.Parameters.AddWithValue("@account_id", account.AccountId);
        cmd.Parameters.AddWithValue("@amount", account.Amount);
        cmd.Parameters.AddWithValue("@type", account.Type.ToString());

        cmd.ExecuteNonQuery();
    }
}
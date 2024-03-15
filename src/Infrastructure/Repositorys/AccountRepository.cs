using Core.Account;
using Core.Servises.UserService;
using Core.Transaction;
using Npgsql;

namespace Infrastructure;

public class AccountRepository : IAccountRepository
{
    private const decimal AdminAccountId = 1;
    private readonly string _connectionString;

    public AccountRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public AccountUser GetById(decimal id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", connection);
        cmd.Parameters.AddWithValue("@id", id);
        using NpgsqlDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) throw new InvalidOperationException("Account not found.");

        string type = reader.GetString(reader.GetOrdinal("type"));
        AccountType accountType;
        if (!Enum.TryParse(type, out accountType))
            throw new InvalidOperationException("Unknown account type");

        switch (accountType)
        {
            case AccountType.User:
                return new StandartAccountUser() { Id = reader.GetDecimal(reader.GetOrdinal("id")), Balance = reader.GetDecimal(reader.GetOrdinal("balance")), Type = accountType };
            case AccountType.Admin:
                return new StandartAccountUser() { Id = reader.GetDecimal(reader.GetOrdinal("id")), Balance = reader.GetDecimal(reader.GetOrdinal("balance")), Type = accountType };
            default:
                throw new InvalidOperationException("Invalid account type");
        }
    }

    public void SaveUser(AccountUser account)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string updateCommand = @"
        UPDATE users
        SET   type = @type,
            balance = @balance
        WHERE id = @id;";

        using var cmd = new NpgsqlCommand(updateCommand, connection);
        cmd.Parameters.AddWithValue("@id", account.Id);
        cmd.Parameters.AddWithValue("@type", account.Type.ToString());
        cmd.Parameters.AddWithValue("@balance", account.Balance);

        int affectedRows = cmd.ExecuteNonQuery();
        if (affectedRows == 0)
            throw new InvalidOperationException("Update failed, account not found.");
    }

    public void CreateUser(AccountUser account)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string insertCommand = @"
        INSERT INTO users (type, balance, pin_code)
        VALUES (@type, @balance, @pin_code)
        RETURNING id;";

        using var cmd = new NpgsqlCommand(insertCommand, connection);
        cmd.Parameters.AddWithValue("@type", account.Type.ToString());
        cmd.Parameters.AddWithValue("@balance", account.Balance);
        cmd.Parameters.AddWithValue("@pin_code", account.PinCode ?? throw new ArgumentNullException(nameof(account.PinCode), "PinCode cannot be null"));

        object? result = cmd.ExecuteScalar();
        if (result is int id) account.Id = (decimal)id;
        else throw new InvalidOperationException("Id cannot be null or non-integer.");
    }

    public void DeleteUser(decimal id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string deleteCommand = "DELETE FROM users WHERE id = @id;";

        using var cmd = new NpgsqlCommand(deleteCommand, connection);
        cmd.Parameters.AddWithValue("@id", id);

        int affectedRows = cmd.ExecuteNonQuery();
        if (affectedRows == 0)
            throw new InvalidOperationException("Delete failed, account not found.");
    }

    public bool ValidatePassword(decimal accountId, string pinCode)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string command = "SELECT pin_code FROM users WHERE id = @id;";
        using var cmd = new NpgsqlCommand(command, connection);
        cmd.Parameters.AddWithValue("@id", accountId);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) return false;
        string storedPinCode = reader.GetString(0);
        return storedPinCode == pinCode;
    }

    public IEnumerable<TransactionATM> GetTransactionsByAccountId(decimal accountId)
    {
        var transactions = new List<TransactionATM>();

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string command = "SELECT * FROM transactions WHERE account_id = @account_id;";
        using var cmd = new NpgsqlCommand(command, connection);
        cmd.Parameters.AddWithValue("@account_id", accountId);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string transactionTypeString = reader.GetString(reader.GetOrdinal("type"));
            if (Enum.TryParse(transactionTypeString, out TransactionType transactionType))
            {
                var transaction = new StandartTransactionATM()
                {
                    Id = reader.GetDecimal(reader.GetOrdinal("id")),
                    AccountId = reader.GetDecimal(reader.GetOrdinal("account_id")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                    Type = transactionType,
                };
                transactions.Add(transaction);
            }
            else
            {
                throw new AggregateException("Unknown transaction type");
            }
        }

        return transactions;
    }

    public bool ValidatePassword(string password)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        const string command = "SELECT pin_code FROM users WHERE id = @id;";
        using var cmd = new NpgsqlCommand(command, connection);
        cmd.Parameters.AddWithValue("@id", AdminAccountId);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) return false;
        string storedAdminPassword = reader.GetString(0);
        return storedAdminPassword == password;
    }
}
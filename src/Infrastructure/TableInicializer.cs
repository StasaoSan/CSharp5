using Npgsql;

namespace Infrastructure;

public class TableInicializer
{
    private readonly string _connectionString;

    public TableInicializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        InitializeWithDefaultData();
        CreateUsersTable(connection);
        CreateTransactionsTable(connection);
    }

    public void InitializeWithDefaultData()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE type = 'Admin'", connection);
        object? result = checkCmd.ExecuteScalar();
        int count = (result as int?) ?? 0;

        if (count == 0)
        {
            using var insertCmd = new NpgsqlCommand("INSERT INTO users (type, balance, pin_code) VALUES ('Admin', 0, '12345')", connection);
            insertCmd.ExecuteNonQuery();
        }
    }

    private static void CreateUsersTable(NpgsqlConnection connection)
    {
        const string createUsersTableCommand = @"
                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    type VARCHAR(255),
                    balance DECIMAL,
                    pin_code VARCHAR(255)
                );";
        using var cmdUsers = new NpgsqlCommand(createUsersTableCommand, connection);
        cmdUsers.ExecuteNonQuery();
    }

    private static void CreateTransactionsTable(NpgsqlConnection connection)
    {
        const string createTransactionsTableCommand = @"
            CREATE TABLE IF NOT EXISTS transactions (
                id SERIAL PRIMARY KEY,
                account_id INTEGER REFERENCES users(id),
                amount DECIMAL,
                type VARCHAR(255)
            );";
        using var cmdTransactions = new NpgsqlCommand(createTransactionsTableCommand, connection);
        cmdTransactions.ExecuteNonQuery();
    }
}
using Core.Account;
using Core.Transaction;

namespace Core.Servises.UserService;

public interface IAccountRepository
{
    AccountUser GetById(decimal id);
    void SaveUser(AccountUser account);
    void CreateUser(AccountUser account);
    IEnumerable<TransactionATM> GetTransactionsByAccountId(decimal accountId);
    void DeleteUser(decimal id);
    bool ValidatePassword(decimal accountId, string pinCode);
    bool ValidatePassword(string password);
}
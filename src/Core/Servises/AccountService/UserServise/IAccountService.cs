using Core.Account;
using Core.Transaction;

namespace Core.Servises.AccountService;

public interface IAccountService
{
    AccountUser CreateAccount(AccountUser accountUser);
    AccountUser GetAccount(decimal accountId);
    void Deposit(decimal accountId, decimal amount);
    IEnumerable<TransactionATM> GetTransactions(decimal accountId);
    void Withdraw(decimal accountId, decimal amount);
}
using Core.Account;
using Core.Servises.TransactionService;
using Core.Servises.UserService;
using Core.Transaction;

namespace Core.Servises.AccountService;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionService _transactionService;

    public AccountService(IAccountRepository accountRepository, ITransactionService transactionService)
    {
        _accountRepository = accountRepository;
        _transactionService = transactionService;
    }

    public void Deposit(decimal accountId, decimal amount)
    {
        AccountUser account = _accountRepository.GetById(accountId);
        if (account == null) throw new InvalidOperationException("Account not found.");
        account.Balance += amount;
        _accountRepository.SaveUser(account);

        var transaction = new StandartTransactionATM
        {
            AccountId = accountId,
            Amount = amount,
            Date = DateTime.Now,
            Type = TransactionType.Deposit,
        };
        _transactionService.RecordTransaction(transaction);
    }

    public void Withdraw(decimal accountId, decimal amount)
    {
        AccountUser account = _accountRepository.GetById(accountId);
        if (account == null) throw new InvalidOperationException("Account not found.");
        if (account.Balance < amount) throw new InvalidOperationException("Insufficient funds.");

        account.Balance -= amount;
        _accountRepository.SaveUser(account);

        var transaction = new StandartTransactionATM
        {
            AccountId = accountId,
            Amount = -amount,
            Date = DateTime.Now,
            Type = TransactionType.Withdrawal,
        };
        _transactionService.RecordTransaction(transaction);
    }

    public AccountUser CreateAccount(AccountUser accountUser)
    {
        _accountRepository.SaveUser(accountUser);
        return accountUser;
    }

    public AccountUser GetAccount(decimal accountId)
    {
        return _accountRepository.GetById(accountId);
    }

    public IEnumerable<TransactionATM> GetTransactions(decimal accountId)
    {
        return _accountRepository.GetTransactionsByAccountId(accountId);
    }
}

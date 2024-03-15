using Core.Transaction;

namespace Core.Account;

public abstract class AccountUser
{
    public decimal Id { get; set; }
    public AccountType Type { get; set; }
    public decimal Balance { get; set; }
    public string? PinCode { get; set; }
    public IEnumerable<TransactionATM>? History { get; set; }
}
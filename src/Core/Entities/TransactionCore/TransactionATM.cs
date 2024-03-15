namespace Core.Transaction;

public abstract class TransactionATM
{
    public decimal Id { get; set; }
    public decimal AccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
}
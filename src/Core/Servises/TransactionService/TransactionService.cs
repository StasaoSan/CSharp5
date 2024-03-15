using Core.Transaction;

namespace Core.Servises.TransactionService;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository? transactionRepository)
    {
        if (transactionRepository is null) throw new AggregateException("TransactionRepository cant be null");
        _transactionRepository = transactionRepository;
    }

    public void RecordTransaction(TransactionATM transaction)
    {
        _transactionRepository.Save(transaction);
    }
}
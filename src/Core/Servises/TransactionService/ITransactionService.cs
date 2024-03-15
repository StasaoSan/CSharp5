using Core.Transaction;

namespace Core.Servises.TransactionService;

public interface ITransactionService
{
    void RecordTransaction(TransactionATM transaction);
}
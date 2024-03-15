using Core.Transaction;

namespace Core.Servises.TransactionService;

public interface ITransactionRepository
{
    void Save(TransactionATM account);
}
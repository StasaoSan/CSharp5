using Core.Account;

namespace Core.Servises.AccountService;

public interface IAdminService
{
    void CreateNewAccount(AccountUser accountUser);
    void DeleteAccount(int accountId);
}

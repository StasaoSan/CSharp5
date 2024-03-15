using Core.Account;
using Core.Servises.UserService;

namespace Core.Servises.AccountService;

public class AdminService : IAdminService
{
    private readonly IAccountRepository _accountRepository;

    public AdminService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public void CreateNewAccount(AccountUser accountUser)
    {
        _accountRepository.CreateUser(accountUser);
    }

    public void DeleteAccount(int accountId)
    {
        _accountRepository.DeleteUser(accountId);
    }
}
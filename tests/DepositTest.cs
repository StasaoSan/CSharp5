using Core.Account;
using Core.Servises.AccountService;
using Core.Servises.TransactionService;
using Core.Servises.UserService;
using Core.Transaction;
using NSubstitute;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab5.Tests;

public class DepositTest
{
    [Fact]
    public void DepositIncreasesBalanceCorrectly()
    {
        IAccountRepository mockRepo = Substitute.For<IAccountRepository>();
        ITransactionService mockTransactionService = Substitute.For<ITransactionService>();
        var account = new StandartAccountUser() { Balance = 100 };
        decimal accountId = 1;
        mockRepo.GetById(accountId).Returns(account);
        var accountService = new AccountService(mockRepo, mockTransactionService);

        accountService.Deposit(accountId, 50);

        Assert.Equal(150, account.Balance);
        mockRepo.Received().SaveUser(account);
        mockTransactionService.Received().RecordTransaction(Arg.Is<TransactionATM>(t => t.Amount == 50 && t.AccountId == accountId && t.Type == TransactionType.Deposit));
    }
}
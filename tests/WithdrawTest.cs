using System;
using Core.Account;
using Core.Servises.AccountService;
using Core.Servises.TransactionService;
using Core.Servises.UserService;
using NSubstitute;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab5.Tests;

public class WithDrawTest
{
    [Fact]
    public void WithdrawWithSufficientBalanceShouldDecreaseBalance()
    {
        IAccountRepository mockRepo = Substitute.For<IAccountRepository>();
        ITransactionService mockTransactionService = Substitute.For<ITransactionService>();
        var account = new StandartAccountUser() { Balance = 100 };
        decimal accountId = 1;
        mockRepo.GetById(accountId).Returns(account);
        var accountService = new AccountService(mockRepo, mockTransactionService);

        accountService.Withdraw(accountId, 50);

        Assert.Equal(50, account.Balance);
        mockRepo.Received().SaveUser(account);
    }

    [Fact]
    public void WithdrawWithInsufficientBalanceShouldThrowException()
    {
        IAccountRepository mockRepo = Substitute.For<IAccountRepository>();
        ITransactionService mockTransactionService = Substitute.For<ITransactionService>();
        var account = new StandartAccountUser() { Balance = 30 };
        decimal accountId = 1;
        mockRepo.GetById(accountId).Returns(account);
        var accountService = new AccountService(mockRepo, mockTransactionService);

        Assert.Throws<InvalidOperationException>(() => accountService.Withdraw(accountId, 50));
    }
}

using Application.Parser;
using Core.Account;
using Core.Servises.AccountService;
using Core.Servises.UserService;
using Core.Transaction;
using Infrastructure;

namespace UserUI;

public class UserUI
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountService _accountService;
    private readonly IAdminService _adminService;

    public UserUI(IAccountRepository accountRepository, IAccountService accountService, IAdminService adminService)
    {
        _accountRepository = accountRepository;
        _accountService = accountService;
        _adminService = adminService;
    }

    public void Start(string connectionString)
    {
        var tableInitializer = new TableInicializer(connectionString);
        tableInitializer.Initialize();
        Console.WriteLine("Write command (example: -id 12345 -p password):");
        string? input = Console.ReadLine();
        string[] args = input?.Split(' ') ?? Array.Empty<string>();

        CommandLineArgs parsedArgs = ParseCommandLineArgs(args);
        AccountType accountType = DetermineAccountType(parsedArgs);
        switch (accountType)
        {
            case AccountType.User:
                HandleUserCommands(parsedArgs);
                break;
            case AccountType.Admin:
                HandleAdminCommands();
                break;
            case AccountType.Invalid:
                Console.WriteLine("Invalid account type.");
                break;
            default:
                Console.WriteLine("Unknown account type.");
                break;
        }
    }

    private static CommandLineArgs ParseCommandLineArgs(string[] args)
    {
        var commandLineArgs = new CommandLineArgs();
        var idHandler = new IdHandler();
        var passwordHandler = new PasswordHandler();

        idHandler.SetNext(passwordHandler);

        int position = 0;
        while (position < args.Length)
        {
            idHandler.Handle(args, commandLineArgs, ref position);
            position++;
        }

        return commandLineArgs;
    }

    private AccountType DetermineAccountType(CommandLineArgs args)
    {
        if (string.IsNullOrEmpty(args.Id)) return AccountType.Invalid;
        if (args.Id.Equals("1", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(args.Password))
            return _accountRepository.ValidatePassword(args.Password) ? AccountType.Admin : AccountType.Invalid;
        if (decimal.TryParse(args.Id, out decimal accountId) && !string.IsNullOrEmpty(args.Password))
            return _accountRepository.ValidatePassword(accountId, args.Password) ? AccountType.User : AccountType.Invalid;
        return AccountType.Invalid;
    }

    private void HandleUserCommands(CommandLineArgs parsedArgs)
    {
        if (!decimal.TryParse(parsedArgs.Id, out decimal accountId))
        {
            Console.WriteLine("Invalid account ID.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\nChoose operation:");
            Console.WriteLine("1. Check balance");
            Console.WriteLine("2. Withdrew from account");
            Console.WriteLine("3. Deposit to account");
            Console.WriteLine("4. View history operation");
            Console.WriteLine("5. Exit");

            string? operation = Console.ReadLine();
            switch (operation)
            {
                case "1":
                    AccountUser account = _accountService.GetAccount(accountId);
                    Console.WriteLine($"Balance account: {account.Balance}");
                    break;
                case "2":
                    Console.WriteLine("Write sum for withdraw:");
                    if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                    {
                        try
                        {
                            _accountService.Withdraw(accountId, withdrawAmount);
                            Console.WriteLine("Success withdraw from you account");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect sum");
                    }

                    break;
                case "3":
                    Console.WriteLine("Write sum for deposit:");
                    if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                    {
                        _accountService.Deposit(accountId, depositAmount);
                        Console.WriteLine("Success deposit to your account");
                    }
                    else
                    {
                        Console.WriteLine("Incorrect sum");
                    }

                    break;
                case "4":
                    IEnumerable<TransactionATM> transactions = _accountService.GetTransactions(accountId);
                    foreach (TransactionATM transaction in transactions)
                        Console.WriteLine($"ID: {transaction.Id}, Sum: {transaction.Amount}, Type: {transaction.Type}");
                    break;

                case "5":
                    return;
                default:
                    Console.WriteLine("Incorect operation, pls be careful, and try again");
                    break;
            }
        }
    }

    private void HandleAdminCommands()
    {
        while (true)
        {
            Console.WriteLine("\nChoose operation:");
            Console.WriteLine("1. Create new account");
            Console.WriteLine("2. Delete account");
            Console.WriteLine("3. Exit");

            string? operation = Console.ReadLine();
            switch (operation)
            {
                case "1":
                    CreateNewAccount();
                    break;
                case "2":
                    DeleteAccount();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Incorect operation, pls be careful, and try again");
                    break;
            }
        }
    }

    private void CreateNewAccount()
    {
        const decimal balance = 0;
        Console.WriteLine("Write account type (User/Admin):");
        string? accountTypeInput = Console.ReadLine();
        AccountType accountType;
        if (!Enum.TryParse(accountTypeInput, true, out accountType))
        {
            Console.WriteLine("Incorrect account type");
            return;
        }

        Console.WriteLine("Write pin-code:");
        string? pinCode = Console.ReadLine();

        var accountUser = new StandartAccountUser { Type = accountType, Balance = balance, PinCode = pinCode };
        _adminService.CreateNewAccount(accountUser);

        Console.WriteLine($"Account with ID {accountUser.Id} has successfully added");
    }

    private void DeleteAccount()
    {
        Console.WriteLine("Write ID account to delete:");
        if (!int.TryParse(Console.ReadLine(), out int accountId))
        {
            Console.WriteLine("Incorrect ID account");
            return;
        }

        try
        {
            _adminService.DeleteAccount(accountId);
            Console.WriteLine($"Account with ID {accountId} deleted succesfully");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"Operation cant be execute: {ex.Message}");
        }
    }
}
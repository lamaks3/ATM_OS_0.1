namespace ATM_OS;
using System;
public class DataBaseQueries
{
    public static void AddToBalance(CardHolderRepository repository,string uid)
    {
        Console.Write("Input sum to deposit: ");
        int amount = int.Parse(Console.ReadLine());

        if (!repository.AddToBalance(uid, amount))
        {
            Console.WriteLine("Invalid amount!");
            return;
        }
        
        Console.WriteLine($"Balance deposited succesfully. Total: {repository.GetBalance(uid)} {repository.GetCurrency(uid)}");
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static void GetBalance(CardHolderRepository repository,string uid)
    {
        double balance = repository.GetBalance(uid);
        Console.WriteLine($"Balance: {balance} "+repository.GetCurrency(uid));
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    
    public static void SubtractFromBalance(CardHolderRepository repository,string uid)
    {
        Console.Write("Введите сумму для снятия: ");
        int amount = int.Parse(Console.ReadLine());
        
        if (!repository.AddToBalance(uid, -amount))
        {
            Console.WriteLine("Invalid amount!");
            return;
        }
        
        double newBalance = repository.GetBalance(uid);
        Console.WriteLine($"Средства успешно сняты. Новый баланс: {newBalance}");
        
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    public static bool IsCardExist(CardHolderRepository repository,string cardUid)
    {
        return repository.CardExists(cardUid);
    }
    
    public static void GetName(CardHolderRepository repository, string uid)
    {
        var holder = repository.GetCardHolderByUid(uid);
        Console.WriteLine($"Имя: {holder.HolderName}");
    }

    public static bool IsPasswordValid(CardHolderRepository repository, string uid,string password)
    {
        return repository.checkPIN(uid, password);
    }
}
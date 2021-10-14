using PaymentGateway.Models;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Data
{
    public class Database
    {
        public List<Person> Persons = new();
        public List<Account> Accounts = new();
        public List<Product> Products = new();
        public List<Transaction> Transactions = new();
        public List<ProductXTransaction> ProductXTransactions = new();

        //private static Database _instance;

        //public static Database GetInstance()
        //{
        //    if(_instance == null)
        //    {
        //        _instance = new Database();
        //    }
        //    return _instance;
        //}

        public static void SaveChanges()
        {
            Console.WriteLine("Save changes to database");
        }
    }
}

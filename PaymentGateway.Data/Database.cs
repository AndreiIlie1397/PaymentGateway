using PaymentGateway.Models;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Data
{
    public class Database
    {
        public List<Person> Persons = new List<Person>();
        public List<Account> Accounts = new List<Account>();
        public List<Product> Products = new List<Product>();
        public List<Transaction> Transactions = new List<Transaction>();
        public List<ProductXTransaction> ProductXTransactions = new List<ProductXTransaction>();

        private static Database _instance;

        //public static Database GetInstance()
        //{
        //    if(_instance == null)
        //    {
        //        _instance = new Database();
        //    }
        //    return _instance;
        //}

        public void SaveChanges()
        {
            Console.WriteLine("Save changes to database");
        }
    }
}

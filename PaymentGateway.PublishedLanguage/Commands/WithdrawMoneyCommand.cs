﻿using MediatR;
using System;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class WithdrawMoneyCommand : IRequest
    {
        public int? AccountId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Iban { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public DateTime DateOfOperation { get; set; }
    }
}

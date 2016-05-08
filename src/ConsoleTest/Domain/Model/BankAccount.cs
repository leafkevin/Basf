using Basf;
using Basf.Domain;
using ConsoleTest.Domain.Event;
using System;
using System.Threading.Tasks;

namespace ConsoleTest.Domain.Model
{
    public class BankAccount : AggRoot<int>
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal Locked { get; set; }
        public BankAccount(int accountId, decimal balance)
            : base(accountId)
        {
            this.Balance = balance;
        }        
        public async Task Deposit(string commandId, decimal amount)
        {
            await this.ApplyChange(new BankAccountAmountLocked(commandId, this.UniqueId, amount));
        }
        public async Task Withdraw(string commandId, decimal amount)
        {
            await this.ApplyChange(new BankAccountAmountLocked(commandId, this.UniqueId, amount));
        }
        public async Task Transfer(string commandId, decimal amount)
        {
            await this.ApplyChange(new BankAccountAmountLocked(commandId, this.UniqueId, amount));
        }
        public async Task LockAmount(string commandId, decimal amount)
        {
            if (this.Balance < amount)
            {
                throw new Exception("余额不足，无法锁定");
            }
            await this.ApplyChange(new BankAccountAmountLocked(commandId, this.UniqueId, amount));
        }
        public void Handle(BankAccountAmountLocked domainEvent)
        {
            this.Balance -= domainEvent.Amount;
            this.Locked = domainEvent.Amount;
        }
    }
}

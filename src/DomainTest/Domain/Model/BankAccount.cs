using Basf.Data;
using Basf.Domain;
using DomainTest.Domain.Event;
using DomainTest.Domain.ValueObject;
using System.Threading.Tasks;

namespace DomainTest.Domain.Model
{
    public class BankAccount : AggRoot<int>
    {
        #region 公共属性
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public bool IsLocked { get; set; }
        public decimal Frozen { get; set; }
        #endregion

        #region 构造方法
        public BankAccount(int accountId, int userId, decimal balance)
            : base(accountId)
        {
            this.UserId = userId;
            this.Balance = balance;
        }
        #endregion

        #region 公共方法
        public async Task<ActionResponse> Lock(string commandId)
        {
            return await this.ApplyChange(new BankAccountLocked(commandId, this.UniqueId));
        }
        public async Task<ActionResponse> Unlock(string commandId)
        {
            return await this.ApplyChange(new BankAccountUnlocked(commandId, this.UniqueId));
        }
        public async Task<ActionResponse> Deposit(string commandId, decimal amount)
        {
            if (this.IsLocked)
            {
                return ActionResponse.Fail((int)BusinessErrors.AccountLocked, "账户已锁定，无法充值");
            }
            return await this.ApplyChange(new BankAccountDeposited(commandId, this.UniqueId, amount));
        }
        public async Task<ActionResponse> Withdraw(string commandId, decimal amount)
        {
            if(this.IsLocked)
            {
                return ActionResponse.Fail((int)BusinessErrors.AccountLocked, "账户已锁定，无法提现");
            }
            if (this.Balance < amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法提现");
            }
            return await this.ApplyChange(new BankAccountWithdrawed(commandId, this.UniqueId, amount));
        }
        public async Task<ActionResponse> Transfer(string commandId, int transactionId, decimal amount)
        {
            if (this.IsLocked)
            {
                return ActionResponse.Fail((int)BusinessErrors.AccountLocked, "账户已锁定，无法转账");
            }
            if (this.Balance < amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法转账");
            }
            return await this.ApplyChange(new BankAccountTransferred(commandId, transactionId, this.UniqueId, amount));
        }
        public async Task<ActionResponse> Accept(string commandId, int transactionId, decimal amount)
        {
            if (this.IsLocked)
            {
                return ActionResponse.Fail((int)BusinessErrors.AccountLocked, "账户已锁定，无法接受汇款");
            }
            return await this.ApplyChange(new BankAccountAccepted(commandId, transactionId, this.UniqueId, amount));
        }
        public async Task<ActionResponse> FreezeAmount(string commandId, decimal amount)
        {
            if (this.IsLocked)
            {
                return ActionResponse.Fail((int)BusinessErrors.AccountLocked, "账户已锁定，无法冻结资金");
            }
            if (this.Balance < amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法锁定");
            }
            return await this.ApplyChange(new BankAccountAmountFrozen(commandId, this.UniqueId, amount));
        }
        public async Task<ActionResponse> CommitFreezeAmount(string commandId, decimal amount)
        {
            if (this.IsLocked)
            {
                return ActionResponse.Fail((int)BusinessErrors.AccountLocked, "账户已锁定，无法冻结资金");
            }
            if (this.Balance < amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法锁定");
            }
            return await this.ApplyChange(new BankAccountAmountFrozen(commandId, this.UniqueId, amount));
        }
        #endregion

        #region Handle方法
        public ActionResponse Handle(BankAccountDeposited domainEvent)
        {
            this.Balance += domainEvent.Amount;
            return ActionResponse.Success;
        }
        public ActionResponse Handle(BankAccountWithdrawed domainEvent)
        {
            if (this.Balance < domainEvent.Amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法提现");
            }
            this.Balance -= domainEvent.Amount;
            return ActionResponse.Success;
        }
        public ActionResponse Handle(BankAccountTransferred domainEvent)
        {
            if (this.Balance < domainEvent.Amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法转账");
            }
            this.Balance -= domainEvent.Amount;
            return ActionResponse.Success;
        }
        public ActionResponse Handle(BankAccountAccepted domainEvent)
        {
            this.Balance += domainEvent.Amount;
            return ActionResponse.Success;
        }
        public ActionResponse Handle(BankAccountAmountFrozen domainEvent)
        {
            if (this.Balance < domainEvent.Amount)
            {
                return ActionResponse.Fail((int)BusinessErrors.BalanceLacked, "余额不足，无法锁定");
            }
            this.Balance -= domainEvent.Amount;
            this.Frozen = domainEvent.Amount;
            return ActionResponse.Success;
        }
        #endregion
    }
}

using Basf.Data;
using Basf.Domain;
using DomainTest.Domain.Event;
using DomainTest.Domain.ValueObject;
using System;
using System.Threading.Tasks;

namespace DomainTest.Domain.Model
{
    public class BankAccountTransaction : AggRoot<int>
    {
        #region 公共属性
        public int TransferAccountId { get; set; }
        public int AcceptAccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public string CreateAt { get; private set; }
        public string UpdateAt { get; private set; }
        #endregion

        #region 构造方法
        public BankAccountTransaction(int transactionId, int transferAccountId, int acceptAccountId, decimal amount)
            : base(transactionId)
        {
            this.TransferAccountId = transferAccountId;
            this.AcceptAccountId = acceptAccountId;
            this.Amount = amount;
            this.Status = TransactionStatus.Created;
            this.CreateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.UpdateAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion

        #region 公共方法
        public async Task<ActionResponse> Start(string commandId)
        {
            return await this.ApplyChange(new TransactionStarted(commandId, this.UniqueId, this.TransferAccountId, this.AcceptAccountId, this.Amount));
        }
        public async Task<ActionResponse> UpdateState(string commandId, TransactionStatus status)
        {
            return await this.ApplyChange(new TransactionStateUpdated(commandId, this.UniqueId, status));
        }
        #endregion

        #region Handle方法
        public ActionResponse Handle(TransactionStarted domainEvent)
        {
            this.TransferAccountId = domainEvent.TransferAccountId;
            this.AcceptAccountId = domainEvent.AcceptAccountId;
            this.Amount = domainEvent.Amount;
            this.Status = domainEvent.Status;
            this.CreateAt = domainEvent.CreateAt;
            this.UpdateAt = domainEvent.UpdateAt;
            return ActionResponse.Success;
        }
        public ActionResponse Handle(TransactionStateUpdated domainEvent)
        {
            this.Status = domainEvent.Status;
            this.UpdateAt = domainEvent.UpdateAt;
            return ActionResponse.Success;
        }
        #endregion
    }
}

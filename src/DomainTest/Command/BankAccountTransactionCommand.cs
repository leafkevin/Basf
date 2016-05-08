using Basf.Domain.Command;

namespace DomainTest.Command
{
    public class BankAccountTransactionCommand : AbstractCommand
    {
        public int TransferAccountId { get; set; }
        public int AcceptAccountId { get; set; }
        public decimal Amount { get; set; }
        public BankAccountTransactionCommand(int transferAccountId, int acceptAccountId, decimal amount)
        {
            this.TransferAccountId = transferAccountId;
            this.AcceptAccountId = acceptAccountId;
            this.Amount = amount;
        }
    }
}

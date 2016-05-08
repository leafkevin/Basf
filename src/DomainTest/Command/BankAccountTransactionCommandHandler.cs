using Basf.Domain.Command;
using System.Threading.Tasks;
using Basf.Data;
using Basf;
using Basf.Domain.Repository;
using DomainTest.Domain.Model;
using DomainTest.Domain.ValueObject;

namespace DomainTest.Command
{
    public class BankAccountTransactionCommandHandler : ICommandHandler<BankAccountTransactionCommand>
    {
        public async Task<ActionResponse> Execute(BankAccountTransactionCommand command)
        {
            var repository = AppRuntime.Resolve<IRepository<BankAccount>>();
            var account1 = await repository.GetAsync(new { UniqueId = 1 });
            var account2 = await repository.GetAsync(new { UniqueId = 2 });
            var transaction = new BankAccountTransaction(1, 1, 2, 50);
            var result = await account1.Transfer(command.UniqueId, transaction.UniqueId, transaction.Amount);
            if (result.Result == ActionResult.Failed)
            {
                await transaction.UpdateState(command.UniqueId, TransactionStatus.Canceled);
                return result;
            }
            await transaction.UpdateState(command.UniqueId, TransactionStatus.Transferred);
            result = await account2.Accept(command.UniqueId, transaction.UniqueId, transaction.Amount);
            if (result.Result == ActionResult.Failed)
            {
                await account1.Transfer(command.UniqueId, transaction.UniqueId, transaction.Amount);
                await transaction.UpdateState(command.UniqueId, TransactionStatus.Canceled);
                return result;
            }
            return await transaction.UpdateState(command.UniqueId, TransactionStatus.Commited);
        }
    }
}

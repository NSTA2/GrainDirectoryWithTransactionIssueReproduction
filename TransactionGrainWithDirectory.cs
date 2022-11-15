using Orleans.GrainDirectory;
using Orleans.Transactions.Abstractions;

namespace GrainDirectoryWithTransactionIssueReproduction
{
    [GrainDirectory("dir")]
    internal class TransactionGrainWithDirectory : ITransactionGrainWithDirectory
    {
        private readonly ITransactionalState<TransactionalGrainState> state;
        public TransactionGrainWithDirectory([TransactionalState("tg", "transactional-table")] ITransactionalState<TransactionalGrainState> transactionalState)
        {
            this.state = transactionalState;
        }

        public async Task<string> SayHi()
        {
            int num = 0;
            await state.PerformUpdate(state =>
            {
                state.HelloNumber++;
                num = state.HelloNumber;
            });

            return $"Hello number {num}";
        }
    }
}

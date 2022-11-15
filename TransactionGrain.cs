using Orleans.Transactions.Abstractions;

namespace GrainDirectoryWithTransactionIssueReproduction
{
    internal class TransactionGrain : ITransactionGrain
    {
        private readonly ITransactionalState<TransactionalGrainState> state;
        public TransactionGrain([TransactionalState("tg", "transactional-table")] ITransactionalState<TransactionalGrainState> transactionalState)
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

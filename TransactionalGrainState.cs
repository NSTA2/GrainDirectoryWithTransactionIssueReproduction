using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainDirectoryWithTransactionIssueReproduction
{
    [GenerateSerializer]
    internal class TransactionalGrainState
    {
        [Id(0)]
        public int HelloNumber { get; set; }
    }
}

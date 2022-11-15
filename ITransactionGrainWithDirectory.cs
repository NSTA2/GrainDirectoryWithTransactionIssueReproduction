using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainDirectoryWithTransactionIssueReproduction
{
    internal interface ITransactionGrainWithDirectory : IGrainWithStringKey
    {
        [Transaction(TransactionOption.Create)]
        Task<string> SayHi();
    }
}

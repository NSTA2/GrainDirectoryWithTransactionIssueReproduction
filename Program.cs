
using GrainDirectoryWithTransactionIssueReproduction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;


var host = await CreateHost();


// Grain 1:  transactions enabled, but no grain directory
var client = host.Services.GetRequiredService<IClusterClient>();
var clientA = client.GetGrain<ITransactionGrain>("A");

Console.WriteLine("No Directory:");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(await clientA.SayHi());
}


// Grain 2:  transactions enabled (with grain directory attribute applied)
Console.WriteLine("With Directory:");
var clientB = client.GetGrain<ITransactionGrainWithDirectory>("B");
for (int i = 0; i < 10; i++)
{
    /*
        The next line fails: the only difference being that the second grain type uses a persisted grain directory

        Orleans.Runtime.OrleansMessageRejectionException: 
        'Forwarding failed: tried to forward message Request [S127.0.0.1:11111:27517279 sys.client/hosted-127.0.0.1:11111@27517279]->[S127.0.0.1:11111:27517279 transactiongrainwithdirectory/B]
        GrainDirectoryWithTransactionIssueReproduction.ITransactionGrainWithDirectoryGrainDirectoryWithTransactionIssueReproduction.ITransactionGrainWithDirectory.SayHi() 
        #28[ForwardCount=2] for 2 times after "Failed to register activation in grain directory." to invalid activation. Rejecting now. '
     
     */
    Console.WriteLine(await clientB.SayHi());
}


await host.StopAsync();


async Task<IHost> CreateHost()
{
    var cnn = "UseDevelopmentStorage=true;";

    var host = new HostBuilder().UseOrleans(sb =>
    {
        sb
        .UseLocalhostClustering()
        .AddAzureTableGrainDirectory("dir", opt => opt.ConfigureTableServiceClient(cnn))
        .AddAzureTableTransactionalStateStorage("transactional-table", opt =>
        {
            opt.TableName = "transactions";
            opt.ConfigureTableServiceClient(cnn);
        })
        .Configure<TransactionalStateOptions>(options =>
        {
            options.LockTimeout = TimeSpan.FromSeconds(2);
        })
        .UseTransactions()
        .Configure<ClusterOptions>(opt =>
        {
            opt.ClusterId = Guid.NewGuid().ToString();
            opt.ServiceId = Guid.NewGuid().ToString();
        })
        .AddMemoryGrainStorageAsDefault();
    }).Build();

    await host.StartAsync();

    return host;
}
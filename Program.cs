
using GrainDirectoryWithTransactionIssueReproduction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

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

var client = host.Services.GetRequiredService<IClusterClient>();
var clientA = client.GetGrain<ITransactionGrain>("A");

Console.WriteLine("No Directory:");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(await clientA.SayHi());
}

Console.WriteLine("With Directory:");
var clientB = client.GetGrain<ITransactionGrainWithDirectory>("B");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(await clientB.SayHi());
}


await host.StopAsync();
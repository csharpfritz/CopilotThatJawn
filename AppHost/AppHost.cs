using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("azure-storage")
	.RunAsEmulator(options =>
	{
		// Configure the Azure Storage Emulator options here if needed
		options.WithLifetime(ContainerLifetime.Persistent);
	});

var tables = storage.AddTables("tables");

var web = builder.AddProject<Web>("web")
	.WithReference(tables)
	.WaitFor(tables)
	.WithExternalHttpEndpoints();

builder.Build().Run();

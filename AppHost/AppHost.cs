using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// NOTE: The type returned by AddAzureStorage is IResourceBuilder<AzureStorageResource>,
// but if storage is being cast or wrapped, ensure it is of the correct type for extension methods.
var storage = builder.AddAzureStorage("azure-storage")
	.RunAsEmulator(options =>
	{
		// Configure the Azure Storage Emulator options here if needed
		options.WithLifetime(ContainerLifetime.Persistent);
		options.WithTablePort(27002);
		options.WithBlobPort(27001);
		options.WithQueuePort(27003);
	});

var tables = storage.AddTables("tables");

var web = builder.AddProject<Web>("web")
	.WithReference(tables)
	.WaitFor(tables)
	.WithExternalHttpEndpoints();

builder.Build().Run();

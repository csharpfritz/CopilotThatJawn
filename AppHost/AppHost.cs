using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apexDomain = builder.AddParameter("domainApex");
var wwwDomain = builder.AddParameter("domainWww");
var certName = builder.AddParameter("certificateName");
var wwwCertName = builder.AddParameter("wwwCertificateName");

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
	.WithExternalHttpEndpoints()
	.PublishAsAzureContainerApp((mod,app) =>
	{
#pragma warning disable ASPIREACADOMAINS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
		app.ConfigureCustomDomain(apexDomain, certName);
		app.ConfigureCustomDomain(wwwDomain, wwwCertName);
#pragma warning restore ASPIREACADOMAINS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
	});

builder.Build().Run();

using Aspire.Hosting;
using Aspire.Hosting.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppHost
{
    internal static class AzureStorageResourceBuilderExtensions
    {
        public static IResourceBuilder<AzureTableStorageResource> WithLoadContentCommand(
            this IResourceBuilder<AzureTableStorageResource> builder)
        {
            builder.WithCommand("load-content","Load Content",
						context => OnRunLoadContentCommandAsync(builder, context),
						new CommandOptions
						{
							UpdateState = OnUpdateResourceState,
							IconName = "DatabaseArrowDown",
							IconVariant = IconVariant.Filled
						});
            return builder;
        }

        private static async Task<ExecuteCommandResult> OnRunLoadContentCommandAsync(
            IResourceBuilder<AzureTableStorageResource> builder,
            ExecuteCommandContext context)
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var connectionString = new ConnectionStringReference(builder.Resource, false);

								logger.LogInformation($"Loading content from directory: {AppContext.BaseDirectory}");
								logger.LogInformation($"Using connection string: {connectionString}");

                var contentDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Content");
                if (!Directory.Exists(contentDir))
                {
                    logger.LogWarning($"Content directory not found: {contentDir}");
                    return CommandResults.Failure($"Content directory not found: {contentDir}");
                }

                var files = Directory.GetFiles(contentDir, "*.md", SearchOption.AllDirectories);
                int successCount = 0;
                foreach (var file in files)
                {
                    try
                    {
                        var tip = ContentUploadHelper.ParseMarkdownFile(file);
                        await ContentUploadHelper.UploadToTableStorage(tip, connectionString.ToString());
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Failed to process file: {file}");
                    }
                }
                logger.LogInformation($"Loaded {successCount} content items into table storage.");
                return CommandResults.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error running load-content command");
                return CommandResults.Failure($"Error: {ex.Message}");
            }
        }
        private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updating resource state: {ResourceSnapshot}", context.ResourceSnapshot);
            }
            return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled;
        }
    }
}

# Build and run ContentLoader to upload markdown files to Azure Storage Emulator
# Usage: ./upload-content.ps1

# Set strict mode
Set-StrictMode -Version Latest

# Define paths
$solutionRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$contentLoaderPath = Join-Path $solutionRoot 'ContentLoader'
$contentPath = Join-Path $solutionRoot 'Content'

# Build ContentLoader
Write-Host 'Building ContentLoader...'
dotnet build $contentLoaderPath --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Error 'Build failed. Exiting.'
    exit 1
}

# Set Azure Storage Emulator connection string (using ports from AppHost.cs)
$storageEmulatorConnectionString = 'DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;TableEndpoint=http://127.0.0.1:27002/devstoreaccount1;'

# Export connection string for ContentLoader (if it reads from env)
$env:AZURE_STORAGE_CONNECTION_STRING = $storageEmulatorConnectionString

# Run ContentLoader to upload markdown files
Write-Host 'Running ContentLoader to upload markdown files...'
dotnet run --project $contentLoaderPath -- $contentPath

Write-Host 'Content upload complete.'

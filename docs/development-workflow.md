# Development Workflow

This guide explains the recommended development workflow for contributing to the "Copilot That Jawn" project.

## Development Environment Setup

### Prerequisites

- **.NET SDK**: Version 9.0 or higher
- **IDE**: Visual Studio 2022+ or Visual Studio Code with C# extensions
- **Git**: For version control

### Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/CopilotThatJawn.git
   cd CopilotThatJawn
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run the application using .NET Aspire:
   ```bash
   dotnet run --project AppHost
   ```

4. Access the Aspire dashboard at the URL shown in the console output (typically `https://localhost:15888`)
5. Access the main application at `https://localhost:5001` (or the port shown in the Aspire dashboard)

## .NET Aspire Development

The project uses .NET Aspire for service orchestration, which manages Redis, Azure Storage, and the web application.

### Aspire Dashboard

The Aspire dashboard provides:
- **Service Status**: Monitor all running services (Web, Redis, Azure Storage)
- **Logs**: Centralized logging from all services
- **Metrics**: Performance metrics and health checks
- **Resource Management**: View connection strings and service endpoints

### Redis Development

#### Local Redis Setup

Redis runs automatically through Aspire orchestration:

```bash
# Start the full application stack (includes Redis)
dotnet run --project AppHost

# Redis will be available at localhost:6379 (default port)
# RedisInsight dashboard available through Aspire dashboard
```

#### Redis Cache Management

The application provides several ways to interact with the Redis cache:

1. **Through the Application**: Content is automatically cached during normal operation
2. **RedisInsight**: Visual Redis management tool accessible through Aspire dashboard
3. **Cache Refresh API**: Manual cache refresh endpoint

#### Cache Refresh API

```bash
# Refresh cache via API (requires API key in production)
curl -X POST https://localhost:5001/api/cache/refresh \
  -H "X-API-Key: your-api-key"
```

#### Monitoring Redis

- **Aspire Dashboard**: View Redis logs and metrics
- **RedisInsight**: Inspect keys, values, and performance
- **Application Logs**: ContentService logs cache hits/misses

## Using Hot Reload

The project is configured to take advantage of ASP.NET Core's hot reload capabilities through .NET Aspire:

- **Automatic Rebuilding**: When you modify C# files, the application automatically rebuilds
- **Browser Refresh**: Changes to Razor pages, CSS, and JavaScript trigger automatic browser refresh
- **Content Updates**: Adding or modifying content files will be reflected in the application
- **Service Orchestration**: All services (Web, Redis, Storage) are managed together

### Commands

- `dotnet run --project AppHost`: Start the full application stack
- `dotnet watch run --project AppHost`: Start with enhanced hot reload monitoring
- `dotnet test`: Run tests for the entire solution

### Service Dependencies

The Aspire orchestration ensures proper startup order:
1. Redis container starts and becomes ready
2. Azure Storage emulator starts and becomes ready  
3. Web application starts and connects to dependencies

## Code Organization

### Naming Conventions

- **Controllers**: `{Feature}Controller.cs` (e.g., `ToolsController.cs`)
- **Models**: `{Entity}Model.cs`, `{Entity}ViewModel.cs`, `{Entity}Dto.cs`
- **Services**: `I{Service}Service.cs` and `{Service}Service.cs`
- **Pages**: Descriptive names matching URL structure
- **CSS Classes**: BEM methodology (block__element--modifier)

### Best Practices

- Use dependency injection for services
- Follow async/await patterns for I/O operations
- Implement proper error handling and logging
- Use strongly-typed views and models
- Keep controllers and page models slim, move business logic to services

## Content Workflow

1. **Create**: Add new content files to the appropriate directory in `Content/`
2. **Test**: Run the application and verify the content appears correctly
3. **Refine**: Update content based on how it looks on the site
4. **Commit**: Commit changes to version control

## Adding New Features

1. **Plan**: Define the feature requirements and scope
2. **Structure**: Decide where the feature fits in the application architecture
3. **Implement**: Develop the feature with appropriate tests
4. **Document**: Update documentation to reflect the new feature
5. **Test**: Verify the feature works as expected
6. **Commit**: Commit the changes to version control

## Deployment Process

The application is designed to be deployed to Azure App Service:

1. **Build**: `dotnet publish -c Release`
2. **Deploy**: Use Azure DevOps, GitHub Actions, or manual deployment to Azure
3. **Verify**: Test the deployed application to ensure it works correctly

## Testing Strategy

- **Unit Tests**: Test business logic and services in isolation
- **Integration Tests**: Test controllers and data access components together
- **End-to-End Tests**: Test critical user workflows
- **Performance Testing**: Ensure content-heavy pages load quickly

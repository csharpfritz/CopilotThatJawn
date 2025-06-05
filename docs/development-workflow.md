# Development Workflow

This guide explains the recommended development workflow for contributing to the "Copilot That Jawn" project.

## Development Environment Setup

### Prerequisites

- **.NET SDK**: Version 9.0 or higher
- **IDE**: Visual Studio 2022+ or Visual Studio Code with C# extensions
- **Git**: For version control

### Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/CopilotThatJawn.git
   cd CopilotThatJawn
   ```

2. Restore packages:
   ```
   dotnet restore
   ```

3. Run the application with hot reload:
   ```
   dotnet watch run --project Web
   ```

4. Access the site at `https://localhost:5001` (or the port configured in your environment)

## Using Hot Reload

The project is configured to take advantage of ASP.NET Core's hot reload capabilities:

- **Automatic Rebuilding**: When you modify C# files, the application automatically rebuilds
- **Browser Refresh**: Changes to Razor pages, CSS, and JavaScript trigger automatic browser refresh
- **Content Updates**: Adding or modifying content files will be reflected in the application

### Commands

- `dotnet watch run --project Web`: Start the application with hot reload
- `dotnet watch test`: Run tests with automatic re-execution when code changes
- `dotnet watch build`: Build with automatic rebuilding when code changes

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

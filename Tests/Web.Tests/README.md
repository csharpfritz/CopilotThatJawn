# Copilot That Jawn - Unit Tests

This directory contains unit tests for the Copilot That Jawn web application using XUnit framework.

## Running Tests

### From Command Line

You can run the tests using the .NET CLI:

```powershell
# Run all tests
dotnet test Tests/Web.Tests

# Run specific test class
dotnet test Tests/Web.Tests --filter "FullyQualifiedName~Web.Tests.Extensions.ImageUrlRewriterExtensionTests"

# Run with detailed output
dotnet test Tests/Web.Tests -v n
```

### From Visual Studio

1. Open the Test Explorer window (Test > Test Explorer)
2. Click "Run All Tests" or right-click on specific tests to run them

## Test Organization

Tests are organized to mirror the structure of the main project:

- `Extensions/` - Tests for extension methods
- `Services/` - Tests for services
- `Controllers/` - Tests for controllers
- `Pages/` - Tests for Razor Pages
- `Components/` - Tests for components

## Best Practices

- Each test class should focus on testing a single component
- Use meaningful test names that describe what is being tested
- Follow the Arrange-Act-Assert pattern
- Use Moq for mocking dependencies
- Keep tests independent of each other

## Adding New Tests

When adding new tests:

1. Create a test class in the appropriate subdirectory
2. Name the test class after the class being tested, with a "Tests" suffix
3. Group related tests within the same test class
4. Use [Fact] for simple tests and [Theory] with [InlineData] for parameterized tests

## Integration with .NET Aspire

When testing components that use .NET Aspire services, use the Aspire testing tools for integration tests involving multiple services.

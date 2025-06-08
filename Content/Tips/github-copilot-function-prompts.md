---
title: "Writing Effective Function Prompts for GitHub Copilot"
category: "GitHub Copilot"
tags: ["prompt-engineering", "best-practices", "functions", "productivity"]
difficulty: "Beginner"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn how to write better function and method prompts to get more accurate suggestions from GitHub Copilot"
---

# Writing Effective Function Prompts for GitHub Copilot

When working with GitHub Copilot, your function names and comments are like headlines - they tell Copilot what kind of code you want to generate. Here's how to make them more effective.

## 1. Use Descriptive Function Names

Your function names should clearly indicate their purpose:

```csharp
// This jawn tells Copilot exactly what we need
public async Task<List<User>> GetActiveUsersFromDatabaseAsync()
{
    // Copilot understands: database query, active users, async operation
}

// Even better - add context in comments
// Returns users who have logged in within the last 30 days
// Sorts by most recent login date
public async Task<List<User>> GetRecentlyActiveUsersAsync()
{
    // Copilot now knows both the operation and the business logic
}
```

## 2. Use Examples in Comments

Show Copilot what you expect by providing examples:

```csharp
// Convert temperature from Fahrenheit to Celsius
// Example: 32°F should return 0°C, 212°F should return 100°C
public double FahrenheitToCelsius(double fahrenheit)
{
    return (fahrenheit - 32) * 5 / 9;
}
```

## 3. Specify Return Types and Parameters

Always be clear about your data types:

```csharp
// Create a function that takes a user ID (string)
// Returns a UserProfile object with basic info
// Throws UserNotFoundException if not found
public async Task<UserProfile> GetUserProfileAsync(string userId)
{
    // Copilot knows the exact types and error cases
}
```

## Best Practices

1. **Use Standard Naming Conventions**
   - PascalCase for C# methods
   - camelCase for JavaScript functions
   - Use async suffix for async methods

2. **Include Error Cases**
   - Mention what exceptions might be thrown
   - Specify validation requirements
   - Document edge cases

3. **Reference Related Components**
   - Mention if using Entity Framework
   - Reference any specific patterns or frameworks
   - Include database context if relevant

Remember, the more context you provide in your function signatures and comments, the more accurate Copilot's suggestions will be!

---

*Want to contribute more tips? Submit a pull request to our GitHub repo and help the Philly dev community level up their AI game!*

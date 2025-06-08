---
title: "Write Better copilot-instructions.md Files Using GitHub Copilot"
description: "Learn how to use GitHub Copilot itself to create and improve your copilot-instructions.md files with better contextual awareness"
category: "GitHub Copilot"
tags: ["github-copilot", "best-practices", "meta-programming", "productivity"]
difficulty: "Intermediate"
author: "GitHub Copilot"
publishedDate: "2025-06-08"
lastModified: "2025-06-08"
---

# Write Better copilot-instructions.md Files Using GitHub Copilot

Want to write instructions for Copilot... using Copilot? Here's how to use GitHub Copilot to help create and refine your `copilot-instructions.md` files. It's like teaching Copilot to teach itself - pretty meta, right?

## Getting Started

1. Create a `.github` folder in your project root if it doesn't exist
2. Create a new `copilot-instructions.md` file inside the `.github` folder
3. Add these key starter comments to guide Copilot:
   ```markdown
   # Project Instructions for GitHub Copilot
   // This file provides instructions for GitHub Copilot AI
   // Project: [Your Project Name]
   // Tech Stack: [List your main technologies]
   ```

## Using Copilot to Define Project Patterns

Let Copilot help you document your patterns. Start with comments like these and watch Copilot suggest relevant patterns:

```markdown
## Coding Patterns
// Document our standard patterns for:
// - Error handling
// - Dependency injection
// - Database access
// - API endpoints
```

Copilot will often suggest common patterns based on your existing codebase.

## Documenting Code Organization

Help Copilot understand your project structure with clear directory descriptions:

```markdown
## Project Structure
// Key directories and their purposes:
/src
  /components    # React components
  /hooks        # Custom React hooks
  /utils        # Utility functions
  /api          # API integration
  /types        # TypeScript types
```

## Teaching Copilot Your Conventions

Use examples to show Copilot your preferred conventions:

```markdown
## Naming Conventions
// Examples of our naming patterns:
// Components: UserProfile.tsx, ProductCard.tsx
// Hooks: useAuthentication.ts, useTheme.ts
// Utils: formatDate.ts, validateInput.ts
// Types: UserTypes.ts, ApiResponse.ts
```

## Leveraging Copilot's Pattern Recognition

1. Let Copilot analyze your existing code:
   ```markdown
   // This project follows these patterns:
   // Example component:
   interface Props {
     title: string;
     onAction: () => void;
   }
   
   export const Component: React.FC<Props> = ...
   ```

2. Describe special cases:
   ```markdown
   ## Special Cases
   // Handle Philly-specific date formats: "June 8th, 2025 at 2pm EST"
   // Use "jawn" appropriately in comments and user-facing text
   // Default to US/Eastern timezone for all dates
   ```

## Pro Tips for Better Instructions

1. **Use Active Voice**: 
   ```markdown
   // ❌ "Functions should be named using camelCase"
   // ✅ "Name functions using camelCase"
   ```

2. **Include Examples**:
   ```markdown
   // Format dates like this:
   formatDate("2025-06-08") // Returns: "June 8, 2025"
   ```

3. **Reference Existing Code**:
   ```markdown
   // Follow the pattern in src/components/Header.tsx
   // See src/utils/validation.ts for error handling examples
   ```

## Maintaining Your Instructions

1. **Regular Updates**: Let Copilot help you keep instructions current:
   ```markdown
   // Last updated: 2025-06-08
   // TODO: Update patterns when new features are added
   // Review and update examples monthly
   ```

2. **Version Control**: Track changes in your instructions:
   ```markdown
   ## Changelog
   // 2025-06: Added TypeScript strict mode requirements
   // 2025-05: Updated React component patterns
   // 2025-04: Initial setup
   ```

## Common Pitfalls to Avoid

- Don't be too rigid - leave room for Copilot's AI to be creative
- Avoid overly complex rules that might confuse the AI
- Don't repeat what's already in your linter or TypeScript config
- Keep examples concise but informative

## Testing Your Instructions

After writing your instructions:

1. Create a new file and let Copilot suggest code
2. Check if the suggestions match your patterns
3. Refine instructions based on results
4. Test with different team members

Remember, good `copilot-instructions.md` files evolve with your project. Use Copilot's suggestions to improve the instructions over time, and don't be afraid to experiment with different formats and approaches to find what works best for your team.

---
title: "Level Up GitHub Copilot with copilot-instructions.md"
description: "Learn how to use copilot-instructions.md files to enhance GitHub Copilot's code suggestions and align them with your project's standards"
category: "GitHub Copilot"
tags: ["github-copilot", "best-practices", "productivity", "coding-standards"]
difficulty: "Beginner"
author: "GitHub Copilot"
publishedDate: "2025-06-08"
lastModified: "2025-06-08"
---

# Level Up GitHub Copilot with copilot-instructions.md

A `copilot-instructions.md` file is an effective way to customize GitHub Copilot's behavior for your project. By adding this file to your repository, you can guide Copilot to follow your project's coding standards, naming conventions, and architectural patterns.

## Setting Up copilot-instructions.md

1. Create a `.github` folder in your project's root directory if it doesn't exist
2. Create a file named `copilot-instructions.md` inside the `.github` folder
3. Add it to source control (it works best when committed to your repository)
4. Include your project-specific instructions in Markdown format

## Key Sections to Include

### Project Overview
```markdown
# Project Name Instructions
This is a [type of project] using [frameworks/languages].
Key architectural patterns include [patterns].
```

### Coding Standards
```markdown
## Coding Standards
- Use PascalCase for class names
- Use camelCase for variables and functions
- Format code using [formatter]
- Follow [specific pattern] for error handling
```

### File Organization
```markdown
## File Organization
- Place interfaces in `/interfaces` directory
- Group related components in feature folders
- Follow [specific pattern] for test files
```

## Example Instructions

Here's a comprehensive example:

```markdown
# Project Coding Instructions

## Overview
This is a React TypeScript application following SOLID principles
and clean architecture patterns.

## Code Style
- Use functional components with hooks
- Implement error boundaries for error handling
- Use TypeScript interfaces for all props
- Follow BEM methodology for CSS classes

## Testing Standards
- Write tests using React Testing Library
- Achieve 80% or higher test coverage
- Mock external dependencies
- Use [arrange-act-assert] pattern

## File Structure
/src
  /components      # Reusable UI components
  /features        # Feature-specific code
  /hooks          # Custom React hooks
  /utils          # Utility functions
  /types          # TypeScript types/interfaces
```

## Best Practices

1. **Be Specific**: The more detailed your instructions, the better Copilot can assist you.

2. **Update Regularly**: Keep your instructions current with project evolution:
   - Add new patterns as they emerge
   - Update deprecated practices
   - Include examples of recent decisions

3. **Include Examples**: Provide code snippets showing preferred patterns:
   ```typescript
   // Preferred component structure
   interface Props {
     title: string;
     onAction: () => void;
   }

   export const Component: React.FC<Props> = ({ title, onAction }) => {
     // Implementation
   };
   ```

4. **Document Conventions**: List naming conventions, file organization, and code structure preferences:
   - File naming (e.g., `kebab-case.ts`, `PascalCase.tsx`)
   - Component organization (e.g., styles with components)
   - Import ordering rules

## Pro Tips

- ✨ Use clear section headers to organize your instructions
- 🔍 Include common use cases and patterns
- 🚫 List anti-patterns to avoid
- 📝 Document architectural decisions
- 🔄 Keep instructions in sync with your linting rules

Remember, Copilot reads these instructions when generating suggestions, helping ensure consistent, high-quality code that matches your project's standards.

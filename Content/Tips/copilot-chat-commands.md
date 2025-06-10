---
title: "Essential GitHub Copilot Chat Commands and Variables"
description: "Master GitHub Copilot Chat with essential commands, chat variables, and expert participants to boost your productivity"
category: "GitHub Copilot"
tags: ["github-copilot", "productivity", "chat-commands", "tips"]
difficulty: "Beginner"
author: "GitHub Copilot"
publishedDate: "2025-06-10"
lastModified: "2025-06-10"
---

# Essential GitHub Copilot Chat Commands and Variables

GitHub Copilot Chat offers powerful commands and variables to help you code more efficiently. This guide covers essential slash commands, chat variables, and specialized chat participants that can enhance your development workflow.

## Quick Reference Guide

### Slash Commands

Start your chat with these useful commands:

```plaintext
/explain - Explain how code works
/fix - Get fixes for problems
/tests - Generate unit tests
/doc - Add documentation comments
/new - Create a new project
```

Example usage:
```plaintext
/explain 
// Your code will be explained in detail

/fix
// Copilot will suggest fixes for highlighted issues

/tests
// Generates comprehensive unit tests for your code
```

### Chat Variables

Include specific context by using these variables:

```plaintext
#file - Include current file content
#selection - Include selected text
#block - Include current code block
#function - Include current function/method
#project - Include project context
```

Example usage:
```plaintext
Explain how #function works
// Explains the function you're currently in

What dependencies does #file have?
// Analyzes current file's dependencies

How can I improve #selection?
// Gets suggestions for selected code
```

### Expert Chat Participants

Engage specialized experts with @ mentions:

```plaintext
@workspace - For project structure and patterns
@terminal - For shell command help
@vscode - For VS Code features help
@azure - For Azure services guidance
```

Example usage:
```plaintext
@workspace What's the best place to add this new feature?
// Gets project-aware suggestions

@terminal How do I set up this development environment?
// Gets terminal command guidance

@azure How do I deploy this to Azure?
// Gets Azure-specific deployment help
```

## Pro Tips

1. **Combine Commands and Variables**
   ```plaintext
   /explain #selection
   // Get detailed explanation of selected code
   ```

2. **Use Project Context**
   ```plaintext
   @workspace Is #file following our project patterns?
   // Gets project-aware code review
   ```

3. **Get Specific Help**
   ```plaintext
   @vscode What's the best way to debug #function?
   // Gets VS Code-specific debugging tips
   ```

## Best Practices

1. **Start Broad, Then Narrow Down**
   - Begin with general queries
   - Use variables to add specific context
   - Engage expert participants for specialized help

2. **Leverage Multiple Tools**
   - Combine slash commands with variables
   - Use expert participants for domain-specific questions
   - Reference multiple files when needed

3. **Maintain Context**
   - Use #project for broader context
   - Reference specific code with #selection or #block
   - Switch experts when changing domains

## Common Scenarios

### Code Review
```plaintext
@workspace Review #file for potential improvements
/fix #selection
@terminal How can I test these changes?
```

### Documentation
```plaintext
/doc #function
@workspace How should I document this API?
```

### Project Architecture
```plaintext
@workspace How does #file fit into our architecture?
@azure What's the best way to structure this for Azure deployment?
```

## Working with Multiple Files
```plaintext
Compare implementation between #file and <filename>
@workspace How are these components connected?
```

## Reference

For a complete reference of all available commands and features, visit the [GitHub Copilot Chat Cheat Sheet](https://docs.github.com/en/copilot/using-github-copilot/copilot-chat/github-copilot-chat-cheat-sheet).

---

*Pro Tip: Keep this guide handy while working with Copilot Chat. The more you use these commands and variables, the more natural and efficient your coding workflow will become.*

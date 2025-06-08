---
title: "JavaScript Frontend AI Integration"
category: "Development"
tags: ["javascript", "frontend", "ai", "integration"]
difficulty: "Intermediate"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn how to integrate AI capabilities into your frontend JavaScript applications with proper error handling and UI feedback."
---

# JavaScript Frontend AI Integration

This guide shows how to implement client-side AI integration with proper loading states, error handling, and user feedback.

## Implementation

```javascript
class AIAssistant {
    constructor(apiEndpoint, apiKey) {
        this.apiEndpoint = apiEndpoint;
        this.apiKey = apiKey;
        this.isLoading = false;
    }

    async generateSuggestion(userInput, context = {}) {
        if (this.isLoading) {
            console.warn('AI request already in progress');
            return null;
        }

        this.isLoading = true;
        this.updateLoadingState(true);

        try {
            const requestBody = {
                prompt: this.buildPrompt(userInput, context),
                max_tokens: 150,
                temperature: 0.5,
                stream: false
            };

            const response = await fetch(`${this.apiEndpoint}/completions`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.apiKey}`
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            return data.choices[0].text.trim();

        } catch (error) {
            console.error('AI suggestion failed:', error);
            this.showErrorMessage('Failed to get AI suggestion. Please try again.');
            return null;
        } finally {
            this.isLoading = false;
            this.updateLoadingState(false);
        }
    }

    buildPrompt(userInput, context) {
        const { projectType = 'general', difficulty = 'beginner' } = context;
        return `
            Context: ${projectType} development project (${difficulty} level)
            User request: ${userInput}
            
            Provide a helpful, specific suggestion:
        `.trim();
    }

    updateLoadingState(isLoading) {
        const button = document.getElementById('ai-suggest-btn');
        const spinner = document.getElementById('loading-spinner');
        
        if (button) {
            button.disabled = isLoading;
            button.textContent = isLoading ? 'Generating...' : 'Get AI Suggestion';
        }
        
        if (spinner) {
            spinner.style.display = isLoading ? 'inline-block' : 'none';
        }
    }

    showErrorMessage(message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger';
        errorDiv.textContent = message;
        
        const container = document.getElementById('ai-responses');
        if (container) {
            container.appendChild(errorDiv);
            setTimeout(() => errorDiv.remove(), 5000);
        }
    }
}

// Example Usage
document.addEventListener('DOMContentLoaded', () => {
    const assistant = new AIAssistant('/api/ai', 'your-api-key-here');
    
    const suggestButton = document.getElementById('ai-suggest-btn');
    if (suggestButton) {
        suggestButton.addEventListener('click', async () => {
            const userInput = document.getElementById('user-input').value;
            const suggestion = await assistant.generateSuggestion(userInput);
            
            if (suggestion) {
                displaySuggestion(suggestion);
            }
        });
    }
});
```

## Key Features

1. **Loading State Management**: Tracks and displays loading states
2. **Error Handling**: Comprehensive error handling with user feedback
3. **UI Feedback**: Visual indicators for loading and error states
4. **Context Support**: Supports additional context in prompts
5. **Rate Limiting**: Prevents multiple simultaneous requests

## Best Practices

- Always show loading states during API calls
- Implement comprehensive error handling
- Provide clear feedback to users
- Prevent multiple simultaneous requests
- Keep API keys secure (preferably on the server side)
- Use proper error boundaries in React applications
- Consider implementing retry logic for failed requests
- Use debouncing for rapid user inputs

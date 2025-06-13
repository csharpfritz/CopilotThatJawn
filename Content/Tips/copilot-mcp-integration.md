---
title: "Getting Started with MCP Servers for Copilot"
description: "A beginner's guide to installing and using MCP servers with GitHub Copilot, featuring the GitHub MCP server as a practical example"
category: "GitHub Copilot"
tags: ["github-copilot", "mcp", "mcp-servers", "beginner", "github-api"]
difficulty: "Beginner"
author: "GitHub Copilot"
publishedDate: "2025-06-11"
lastModified: "2025-06-13"
---

# Getting Started with MCP Servers for Copilot

Model Context Protocol (MCP) servers are pre-built tools that extend GitHub Copilot's capabilities by connecting it to external services and data sources. Instead of building these integrations from scratch, you can install existing MCP servers to instantly give Copilot new superpowers.

## What are MCP Servers?

MCP servers are like plugins that allow Copilot to:

- Access live data from APIs and services
- Perform real-world actions (create issues, search repositories, etc.)
- Use external tools and databases
- Get up-to-date information beyond its training data

Think of MCP servers as giving Copilot the ability to "phone a friend" - connecting it to specialized services that can help answer questions and perform tasks.

## Example: Installing the GitHub MCP Server

The GitHub MCP server is perfect for beginners because it connects Copilot to GitHub's API, allowing you to work with repositories, issues, and pull requests directly in your conversations.

### Prerequisites

Before installing any MCP server, you'll need:

- A GitHub personal access token (for the GitHub MCP server)
- Node.js installed on your system
- Visual Studio Code with the GitHub Copilot extension

### Step 1: Install the GitHub MCP Server

```bash
# Install the GitHub MCP server globally
npm install -g @modelcontextprotocol/server-github

# Or install it locally in your project
npm install @modelcontextprotocol/server-github
```

### Step 2: Configure Your GitHub Token

Create a `.env` file in your project root:

```env
GITHUB_PERSONAL_ACCESS_TOKEN=your_token_here
```

To create a GitHub token:

1. Go to GitHub Settings → Developer settings → Personal access tokens
2. Generate a new token with `repo` and `read:org` permissions
3. Copy the token to your `.env` file

### Step 3: Configure Visual Studio Code Settings

Visual Studio Code users need to configure the MCP server in their settings. You have two options:

#### Option A: User Settings (Recommended for beginners)

Open Visual Studio Code Settings (Ctrl+, or Cmd+,) and add this to your `settings.json`:

```json
{
  "github.copilot.advanced": {
    "debug": true
  },
  "mcp.servers": {
    "github": {
      "command": "node",
      "args": ["./node_modules/@modelcontextprotocol/server-github/dist/index.js"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "${env:GITHUB_PERSONAL_ACCESS_TOKEN}"
      }
    }
  }
}
```

**💡 Visual Studio Code Tip**: Access your settings.json quickly by pressing `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac), then type "Preferences: Open User Settings (JSON)"

#### Option B: Workspace Settings (For project-specific setup)

Create a `.vscode/settings.json` file in your project root with the same configuration. This keeps the MCP server configuration tied to your specific project.

**⚠️ Visual Studio Code Security Note**: Visual Studio Code may show a security prompt when first loading MCP servers. This is normal - click "Allow" to enable the server functionality.

### Step 4: Verify Installation in Visual Studio Code

After configuration, verify everything is working:

1. **Check the Output Panel**:
   - Open View → Output (or `Ctrl+Shift+U`)
   - Select "GitHub Copilot" from the dropdown
   - Look for MCP server connection messages

2. **Test with Copilot Chat**:
   - Open Copilot Chat (Ctrl+Alt+I or click the chat icon)
   - Try asking: "What repositories do I have access to?"

3. **Enable Debug Mode** (if needed):
   - The `"debug": true` setting in your configuration will show detailed logs
   - Check the Visual Studio Code Developer Console (Help → Toggle Developer Tools) for detailed MCP messages

## Using the GitHub MCP Server in Visual Studio Code

Once configured, you can leverage the GitHub MCP server directly within your Visual Studio Code workflow:

### Visual Studio Code Copilot Chat Integration

The GitHub MCP server works seamlessly with Visual Studio Code's Copilot Chat panel:

**🎯 Visual Studio Code Pro Tip**: Use the `#github` context in your Copilot Chat to explicitly reference GitHub data:

```copilot-prompt
"#github What are the recent issues in the microsoft/vscode repository?"
"#github Show me the latest pull requests for my organization's main project"
"#github What's the current status of issue #1234 in my repository?"
```

### Inline Chat with GitHub Context

Visual Studio Code's inline chat (Ctrl+I) can also use GitHub MCP server data:

```copilot-prompt
"Help me write a commit message based on the current GitHub issue I'm working on"
"Generate code comments that reference the GitHub issue this fixes"
"Create a PR description template for this type of change"
```

### Sidebar Integration

Access GitHub functionality through Visual Studio Code's sidebar:

- **Source Control Panel**: Copilot can now suggest commit messages based on related GitHub issues
- **Explorer Panel**: Right-click files and ask Copilot about their GitHub history
- **Problems Panel**: Get suggestions for creating GitHub issues from error patterns

### 1. Repository Information

```copilot-prompt
"What are the recent issues in the microsoft/vscode repository?"
"Show me the latest pull requests for my organization's main project"
"What's the current status of issue #1234 in my repository?"
```

### 2. Creating Issues and PRs

```copilot-prompt
"Create a new issue in my repository about fixing the login bug"
"Draft a pull request description for my authentication improvements"
"Help me find similar issues to the one I'm working on"
```

### 3. Repository Analysis

```copilot-prompt
"What are the most active contributors to this repository?"
"Show me the commit history for the last week"
"What files have been changed most frequently?"
```

## What Makes This Powerful

With the GitHub MCP server connected, Copilot can:

- **Get Real-time Data**: Access current repository state, not just training data
- **Perform Actions**: Create issues, comment on PRs, and update repositories
- **Contextual Suggestions**: Understand your project's GitHub history and patterns
- **Cross-reference**: Link code suggestions to actual issues and discussions

## Other Beginner-Friendly MCP Servers

Once you're comfortable with the GitHub MCP server, try these:

### Database Servers

- **SQLite MCP Server**: Query local databases
- **PostgreSQL MCP Server**: Connect to PostgreSQL databases
- **MongoDB MCP Server**: Work with MongoDB collections

### File System Servers

- **File System MCP Server**: Read and write files
- **Search MCP Server**: Search through project files
- **Git MCP Server**: Perform Git operations

### API Servers

- **REST API MCP Server**: Make HTTP requests to APIs
- **Weather MCP Server**: Get weather data for location-based apps
- **News MCP Server**: Fetch current news for content applications

## Best Practices for Beginners

1. **Start Simple**

   - Begin with one MCP server (like GitHub)
   - Test basic functionality before adding complexity
   - Read the server documentation thoroughly

2. **Secure Your Tokens**

   - Never commit API tokens to version control
   - Use environment variables for sensitive data
   - Regularly rotate your access tokens

3. **Test Your Setup**

   - Verify the MCP server is working with simple queries
   - Check VS Code's developer console for connection issues
   - Start with read-only operations before trying write operations

4. **Understand Permissions**

   - Know what permissions your tokens grant
   - Use minimal required permissions for security
   - Be aware of rate limits and API quotas

## Troubleshooting Common Issues in Visual Studio Code

### MCP Server Not Connecting

**Symptoms**: Copilot doesn't recognize GitHub-related requests

**Visual Studio Code Solutions**:

1. **Check Extension Status**: Ensure GitHub Copilot extension is active in Extensions panel
2. **Reload Window**: Press `Ctrl+Shift+P` → "Developer: Reload Window"
3. **Check Output Panel**: View → Output → Select "GitHub Copilot" for error messages
4. **Verify Settings**: Open `settings.json` and confirm MCP configuration is correct

### Permission Errors in Visual Studio Code

**Symptoms**: "Permission denied" when trying to create issues or access repositories

**Visual Studio Code Solutions**:

1. **Token Verification**:
   - Open Visual Studio Code Terminal (Ctrl+` or View → Terminal)
   - Run: `echo $GITHUB_PERSONAL_ACCESS_TOKEN` to verify token is set
   - If empty, check your `.env` file is in the right location

2. **Extension Permissions**:
   - Go to Extensions → GitHub Copilot → Settings
   - Ensure all necessary permissions are enabled

3. **Workspace Trust**:
   - Visual Studio Code may require you to trust the workspace
   - Click "Trust" when prompted, or go to File → Trust Workspace

### Visual Studio Code Performance Issues

**Symptoms**: Slow responses when using GitHub MCP server

**Visual Studio Code Optimization**:

1. **Disable Unnecessary Extensions**: Turn off extensions you don't need
2. **Increase Memory**: Add to settings.json: `"github.copilot.advanced.length": 2000`
3. **Check CPU Usage**: Open Visual Studio Code's built-in performance monitor (Help → Performance)

**🔧 Visual Studio Code Debug Tip**: Enable verbose logging by adding `"mcp.debug": true` to your settings.json for detailed troubleshooting information.

## Next Steps

Once you're comfortable with the GitHub MCP server:

1. **Explore More Servers**: Try database or file system MCP servers
2. **Combine Servers**: Use multiple MCP servers together for complex workflows
3. **Learn Advanced Features**: Explore server-specific advanced capabilities
4. **Build Custom Servers**: Create your own MCP servers for unique needs (covered in future tips!)

## Learn More

For more information about MCP servers:

- [MCP Server Registry](https://github.com/topics/mcp-server) - Discover available MCP servers
- [GitHub MCP Server Documentation](https://github.com/modelcontextprotocol/servers/tree/main/src/github)
- [MCP Protocol Specification](https://modelcontextprotocol.io/docs/)
- [Visual Studio Code MCP Integration Guide](https://code.visualstudio.com/docs/copilot/mcp-servers)

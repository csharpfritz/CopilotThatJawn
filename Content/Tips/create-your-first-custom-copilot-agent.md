---
title: "Create Your First Custom Copilot Agent: Authoring .agent.md Files for Specialized AI Assistants"
description: "Learn how to create custom Copilot agents using .agent.md files. Build specialized AI assistants with defined roles, boundaries, and tools for your team."
category: "GitHub Copilot"
tags: ["github-copilot", "agents", "agent-md", "customization", "getting-started"]
difficulty: "Beginner"
author: "Copilot That Jawn"
publishedDate: "2026-03-04"
lastModified: "2026-03-04"
series: "Copilot Customization"
part: 1
featured: true
---

# Create Your First Custom Copilot Agent: Authoring .agent.md Files for Specialized AI Assistants

Custom Copilot agents let you create specialized AI assistants that appear in the `@` menu within [Copilot Chat](https://docs.github.com/en/copilot/using-github-copilot/asking-github-copilot-questions-in-your-ide). Instead of one general-purpose assistant, you can build agents with specific roles—like a "code reviewer," "documentation writer," or "database expert"—each with its own expertise, boundaries, and tools.

If you've already explored [`copilot-instructions.md`](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot) (global rules for your whole project) or [`SKILL.md`](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot#creating-a-skill-file) files (reusable playbooks), agents are the next step: they're **named personas that Copilot activates when you call them by name**.

## What's a Custom Copilot Agent?

An agent is a personality layer on top of Copilot. When you invoke it by name, Copilot takes on that role, follows the agent's instructions, respects its boundaries, and uses only the tools you've defined for it.

Think of it like this:

- **`copilot-instructions.md`**: Global rules everyone follows (e.g., "all TypeScript must be strict mode").
- **`SKILL.md`**: Reusable workflows (e.g., "here's how to run our PR checklist").
- **`.agent.md`**: A named specialist persona (e.g., "I'm your security auditor—I review code for vulnerabilities").

Agents are powerful because:
- ✨ **Named and discoverable** — teammates see them in the @ menu
- 🎯 **Focused expertise** — each agent has a clear role and boundaries
- 🔧 **Tool-aware** — you control what commands and tools each agent can use
- 📦 **Teamable** — share them across your repo so everyone gets the same specialist

## Where agents live

Store `.agent.md` files in your repository:

```
your-repo/
└── .github/
    └── agents/
        ├── code-reviewer.agent.md
        ├── docs-writer.agent.md
        └── security-auditor.agent.md
```

Copilot scans `.github/agents/` and makes each agent available by name in chat.

You can also store personal agents in `~/.copilot/agents/` (on your machine only) or share them organization-wide in `.github-private/agents/`.

## The .agent.md file format

Every agent file needs two parts:

1. **YAML frontmatter** (at the top, between `---` markers) — metadata like name, description, and allowed tools
2. **Markdown body** — your agent's persona, rules, examples, and instructions

Here's the structure:

```markdown
---
name: agent-name
description: "What this agent does."
target: github-copilot
tools:
  - npm
  - git
disable-model-invocation: false
---

# Persona
You are a [role]. Your job is to [responsibility].

# Boundaries
Never touch [dangerous places]. Only modify [safe areas].

# Commands
- npm test
- npm run lint

# Code style preferences
...and so on
```

### Frontmatter fields explained

| Field | Required? | Example | Notes |
|-------|-----------|---------|-------|
| `name` | ✅ Yes | `code-reviewer` | Lowercase, hyphens. This is how users invoke the agent (@code-reviewer). |
| `description` | ✅ Yes | "Reviews code for bugs and best practices." | One-liner shown in the @ menu. |
| `target` | ❌ Optional | `github-copilot` | Usually `github-copilot` (Copilot Chat). Defaults to auto-detection. |
| `tools` | ❌ Optional | `["npm", "git", "python"]` | CLI tools/commands this agent can use. Omit for no tool access. |
| `disable-model-invocation` | ❌ Optional | `false` | Set `true` if you only want manual @agent-name invocation (no auto-triggering). |
| `metadata` | ❌ Optional | `owner: dev-team` | Custom key-value pairs for your team's reference. |

### Markdown body best practices

Your agent's instructions live in the Markdown. Include these sections:

- **# Persona** — Who is this agent? What's their specialty?
- **# Boundaries** — What's off-limits? Which files are safe to edit?
- **# Commands** — What CLI commands should the agent know about and use?
- **# Code Style** — Preferences for naming, formatting, patterns.
- **# Examples** — Real sample inputs and outputs so Copilot knows what "good" looks like.

## Step-by-step: create your first agent

Let's build a **"code-reviewer" agent** as an example. This agent will review pull requests and suggest improvements.

### Step 1: Create the file

In your repo, create `.github/agents/code-reviewer.agent.md`:

```bash
mkdir -p .github/agents
touch .github/agents/code-reviewer.agent.md
```

### Step 2: Start with frontmatter

```yaml
---
name: code-reviewer
description: "Reviews pull requests for code quality, bugs, and best practices."
target: github-copilot
tools:
  - git
  - npm
disable-model-invocation: false
metadata:
  owner: dev-team
  version: "1.0"
---
```

This tells Copilot:
- The agent is called `@code-reviewer`
- It reviews code for quality and bugs
- It can use `git` and `npm` commands
- It's enabled for auto-invocation (e.g., when someone asks for a review)

### Step 3: Write the persona

```markdown
# Persona

You are an experienced senior engineer with 10+ years of code review experience. Your job is to:

1. **Find bugs** before they reach production
2. **Suggest improvements** using best practices from our codebase
3. **Mentor** junior engineers through constructive feedback
4. **Enforce standards** without being pedantic

Your tone is friendly, specific, and actionable—never vague or dismissive.
```

### Step 4: Set boundaries

```markdown
# Boundaries

- **Only review code changes**, not documentation (unless asked)
- **Don't suggest refactors** unless they fix a real issue or block understanding
- **Don't approve PRs** with failing tests or unresolved TODOs
- **Never** modify code directly—only suggest patterns and explain the "why"
- **Focus on logic, security, and maintainability**—not whitespace or minor style

## Files you CAN review
- `src/**/*.ts` and `src/**/*.js` — application code
- `lib/**/*.ts` — shared libraries
- Tests in `__tests__/`

## Files you should NOT touch
- Configuration files (`webpack.config.js`, `.eslintrc`, etc.)
- Generated code in `dist/` or `build/`
- Third-party dependencies in `node_modules/`
```

### Step 5: Add a checklist

```markdown
# Code review checklist

When reviewing, use this checklist:

1. **Correctness?**
   - Does the code do what the PR description says?
   - Are there edge cases not handled?

2. **Tests?**
   - Are there tests for the new code?
   - Do they cover happy path AND error cases?
   - Do all tests pass locally?

3. **Performance & security?**
   - Any obvious performance issues?
   - Are external inputs validated?
   - No hardcoded secrets or sensitive data?
```

### Step 6: Add examples

```markdown
# Example review

## Good response format

### ✅ The Good
- Function `calculateDiscount()` is clear and handles edge cases well
- Tests cover both happy path and boundary conditions
- Style matches the rest of the codebase

### 🔍 Questions
- Line 42: Why convert to string here instead of keeping it as a number?
- Is there a test for the negative discount scenario?

### 💡 Suggestion
Consider extracting the validation logic on line 50 into a separate `validateRange()` helper. It would be reusable and easier to test.

## What NOT to do
- Don't just say "looks good" — be specific
- Don't rewrite code unless asked; suggest patterns instead
- Don't approve PRs with failing tests
```

### Step 7: Commit and test

```bash
git add .github/agents/code-reviewer.agent.md
git commit -m "Add code-reviewer agent for PR reviews"
git push
```

Now in Copilot Chat, type `@code-reviewer` and your agent appears. Try asking:

```
@code-reviewer Please review this PR: [paste code changes]
```

## Real-world example from this repository

This repository uses a custom agent called "Squad" for team coordination. You can see it at `.github/agents/squad.agent.md`. It's a more advanced example showing:

- Multi-line frontmatter with metadata
- Detailed persona and responsibilities
- Complex boundaries and refusal rules
- Custom instructions for different session phases

Browse it for inspiration on structuring larger agents.

## Common agent ideas for your team

Here are agents other teams have found useful:

| Agent | Role | Good for |
|-------|------|----------|
| **code-reviewer** | Senior engineer reviewing code | Pull request reviews, quality gates |
| **docs-writer** | Technical writer | Creating/updating documentation |
| **test-writer** | QA engineer | Writing unit and integration tests |
| **database-expert** | DBA/data engineer | Schema design, query optimization |
| **security-auditor** | Security engineer | Vulnerability scanning, threat modeling |
| **performance-tuner** | Optimization specialist | Profiling, caching, load testing |
| **accessibility-checker** | A11y specialist | WCAG compliance, screen reader testing |

## How agents differ from other customizations

| Feature | Global Instructions | Skills | Agents |
|---------|-------|--------|--------|
| **Scope** | Applies to all Copilot interactions in the repo | Pulled in when relevant to the task | Activated by name in chat |
| **How to invoke** | Automatic (always active) | Mentioned in prompts ("use the pr-ready skill") | `@agent-name` in chat |
| **Best for** | Project-wide standards (naming, architecture, coding style) | Detailed, repeatable workflows (testing, deployment, release notes) | Specialized personas with boundaries (reviewers, writers, auditors) |
| **Transparency** | Hidden from users | Discoverable via docs/prompts | Visible in @ menu |

## Pro tips

- 🎯 **Be specific in descriptions** — The one-liner should tell teammates exactly when to use this agent.
- 🚫 **Set clear boundaries** — Tell the agent what it should NOT touch. Security and stability depend on this.
- 🔧 **List tools explicitly** — Only grant access to tools the agent needs. This keeps scope tight and safe.
- 📖 **Include real examples** — Show Copilot what good code review (or documentation, or tests) looks like from your project.
- 🔄 **Version your agents** — If you update an agent significantly, consider adding a version comment at the top.
- 🤝 **Share with your team** — Commit agents to your repo so everyone uses the same specialist.

## What's next?

Now that you understand agents, you can:

1. **Create 2–3 agents** for your team's most common workflows.
2. **Combine with skills** — Have an agent that orchestrates a skill (e.g., "security-auditor" calls your "security-scan" skill).
3. **Integrate with MCP** — If you have the [GitHub MCP server](https://github.com/github/github-mcp-server), agents can fetch live PR data, workflow logs, and more.
4. **Share organization-wide** — Move agents to `.github-private/agents/` so all your org's repos can use them.

## Learn more

For detailed documentation and examples, see these official resources:

- **[GitHub Docs: Creating custom agents](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents)** — Official guide and configuration reference
- **[How to write a great .agent.md](https://github.blog/ai-and-ml/github-copilot/how-to-write-a-great-agents-md-lessons-from-over-2500-repositories/)** — Lessons from 2,500+ agent examples
- **[Copilot agents overview](https://docs.github.com/en/copilot/concepts/agents)** — Understand agents, skills, and customization options

## Related reading in this series

This is Part 1 of the **Copilot Customization** series. Check out:

- **[Level Up GitHub Copilot with copilot-instructions.md](/tips/copilot-instructions-md)** — Project-wide instructions that apply to all Copilot interactions
- **[Create Your First GitHub Copilot Skill](/tips/create-your-first-github-copilot-skill)** — Reusable, step-by-step playbooks for common workflows

All three approaches—instructions, skills, and agents—work together to create a tailored Copilot experience for your team.
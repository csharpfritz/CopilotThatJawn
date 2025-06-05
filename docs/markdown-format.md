# Markdown Content Format

## Introduction

This document describes the Markdown format used for content in "Copilot That Jawn". All content is stored as Markdown files with YAML frontmatter in the `Content/` directory.

## YAML Frontmatter

Each content file begins with YAML frontmatter enclosed between triple-dashes (`---`). This section contains metadata about the content.

### Required Fields

```yaml
---
title: "Your Content Title"
description: "A brief description of your content"
category: "Category Name"
date: "2025-06-05"
---
```

### Optional Fields

```yaml
---
tags: ["tag1", "tag2", "tag3"]
author: "Author Name"
lastModified: "2025-06-05"
slug: "custom-url-slug"
featured: true
image: "/images/featured-image.jpg"
order: 1
relatedContent: ["slug1", "slug2"]
---
```

## Markdown Content

After the frontmatter, write your content using standard Markdown syntax.

### Supported Markdown Features

#### Headings

```markdown
# Heading 1
## Heading 2
### Heading 3
#### Heading 4
##### Heading 5
###### Heading 6
```

#### Text Formatting

```markdown
**Bold text**
*Italic text*
~~Strikethrough text~~
```

#### Lists

```markdown
- Unordered list item 1
- Unordered list item 2
  - Nested item
  - Another nested item
- Unordered list item 3

1. Ordered list item 1
2. Ordered list item 2
   1. Nested ordered item
   2. Another nested ordered item
3. Ordered list item 3
```

#### Links

```markdown
[Link text](https://example.com "Optional title")
```

#### Images

```markdown
![Alt text](/path/to/image.jpg "Optional title")
```

#### Code

Inline code:
```markdown
`code`
```

Code blocks:
````markdown
```language
code block
```
````

#### Blockquotes

```markdown
> This is a blockquote.
> It can span multiple lines.
```

#### Tables

```markdown
| Header 1 | Header 2 | Header 3 |
|----------|----------|----------|
| Cell 1   | Cell 2   | Cell 3   |
| Cell 4   | Cell 5   | Cell 6   |
```

#### Horizontal Rules

```markdown
---
```

### Extended Markdown Features

The site supports several extended Markdown features through the Markdig library:

#### Task Lists

```markdown
- [x] Completed task
- [ ] Incomplete task
```

#### Footnotes

```markdown
Here is some text with a footnote[^1].

[^1]: This is the footnote content.
```

#### Definition Lists

```markdown
Term
: Definition
```

#### Custom Containers

```markdown
:::info
This is an info container
:::

:::warning
This is a warning container
:::

:::tip
This is a tip container
:::
```

## Example Content File

```markdown
---
title: "Getting Started with GitHub Copilot"
description: "Learn how to set up and start using GitHub Copilot effectively in your projects"
category: "Tutorials"
tags: ["github-copilot", "setup", "beginner"]
author: "Developer Name"
date: "2025-05-15"
lastModified: "2025-06-02"
featured: true
image: "/images/github-copilot-setup.jpg"
---

# Getting Started with GitHub Copilot

GitHub Copilot is an AI pair programmer that helps you write better code. This guide will walk you through setting up and using GitHub Copilot effectively.

## Prerequisites

Before you start, make sure you have:

- A GitHub account
- Visual Studio Code, Visual Studio, or JetBrains IDE
- A GitHub Copilot subscription

## Installation Steps

1. Install the GitHub Copilot extension from your IDE's marketplace
2. Sign in with your GitHub account
3. Verify your subscription is active

## Basic Usage

Once installed, you can start using Copilot immediately. Simply start typing, and suggestions will appear:

```javascript
// Example: Start typing a function
function calculateTotal(items) {
  // Copilot will suggest completing this function
}
```

## Tips for Effective Use

- Write clear comments to guide Copilot
- Accept suggestions with Tab key
- Reject suggestions with Esc key
- Use Alt+[ and Alt+] to cycle through alternative suggestions
```

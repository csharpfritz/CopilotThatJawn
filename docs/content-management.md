# Content Management Guide

## Content Organization

"Copilot That Jawn" uses a structured content management approach based on Markdown files with YAML frontmatter. This document explains how content is organized, processed, and displayed on the site.

## Directory Structure

All content is stored in the `Content/` directory at the root of the repository:

```
Content/
├── Tips/               # Individual tip markdown files
├── Tutorials/          # Step-by-step guide markdown files
├── Tools/              # AI tool profile markdown files
├── Guides/             # Best practice and setup guide markdown files
└── News/               # Latest updates and announcements
```

## File Format

Each content file follows this general structure:

```markdown
---
title: "Title of the Content"
description: "A brief description that appears in previews"
category: "Category Name"
tags: ["tag1", "tag2", "tag3"]
author: "Author Name"
date: "2025-01-01"
lastModified: "2025-06-01"
slug: "url-friendly-title"
featured: true|false
image: "/path/to/featured-image.jpg"
---

# Main Content Title

Your markdown content goes here. Regular markdown syntax is supported, including:

- Lists
- **Bold text**
- *Italics*
- [Links](https://example.com)
- Code blocks
- Tables
- And more!

## Subheadings

More content...
```

## YAML Frontmatter Fields

The YAML frontmatter at the top of each Markdown file contains metadata about the content:

| Field | Description | Required |
|-------|-------------|----------|
| title | The title of the content | Yes |
| description | A brief summary | Yes |
| category | Primary category | Yes |
| tags | Array of relevant tags | No |
| author | Content author | No |
| date | Original publication date (YYYY-MM-DD) | Yes |
| lastModified | Last modification date (YYYY-MM-DD) | No |
| slug | URL-friendly identifier (auto-generated if not provided) | No |
| featured | Whether to highlight the content (default: false) | No |
| image | Path to featured image | No |

## Content Processing

The `ContentService` class in the `Web/Services` directory processes these files:

1. **Loading**: Files are loaded from the appropriate content directory
2. **Parsing**: YAML frontmatter is extracted and parsed
3. **Processing**: Markdown content is converted to HTML
4. **Enrichment**: Additional metadata like reading time is calculated
5. **Caching**: Processed content is cached for performance
6. **Serving**: Content is provided to the appropriate pages and controllers

## Adding New Content

To add new content to the site:

1. Create a new Markdown file in the appropriate directory (e.g., `Content/Tips/`)
2. Add the required YAML frontmatter fields
3. Write your content using Markdown
4. The content will automatically appear on the site

## Content Categories

The site currently supports these content categories:

- **Tips**: Quick, actionable advice about AI tools
- **Tutorials**: Step-by-step instructions for accomplishing specific tasks
- **Tools**: Detailed profiles of AI tools, especially in the Microsoft Copilot ecosystem
- **Guides**: Comprehensive coverage of broader topics
- **News**: Updates and announcements about AI tools and features

## Tags

Tags provide cross-cutting organization of content. Some commonly used tags include:

- github-copilot
- microsoft-365-copilot
- azure-ai
- prompt-engineering
- productivity
- code-generation
- natural-language
- philadelphia
- best-practices

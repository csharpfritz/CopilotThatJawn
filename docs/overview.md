# Copilot That Jawn - Project Overview

## Introduction

"Copilot That Jawn" is a comprehensive ASP.NET Core web application that serves as a curated collection of AI tools, tips, tricks, and resources, with a primary focus on the Microsoft Copilot ecosystem. The website is designed to be an educational and resource-rich platform for developers, content creators, and AI enthusiasts to discover and learn about various AI-powered tools and best practices.

## Project Purpose

- **Primary Goal**: Create an educational and resource-rich platform for Microsoft Copilot and AI tools
- **Target Audience**: Developers, content creators, students, and professionals interested in AI productivity tools
- **Content Focus**: Microsoft Copilot (GitHub Copilot, Microsoft 365 Copilot, Copilot Studio), Azure AI services, and complementary AI tools

## Project Structure

The project is structured as an ASP.NET Core 9.0+ application using Razor Pages for the main UI and MVC controllers for more complex operations. Content is primarily stored as Markdown files in the `Content/` directory, which allows for easy content management, version control, and collaboration.

### Key Directories

- **Web/**: Contains the ASP.NET Core application
  - **Pages/**: Razor Pages for the main website
  - **Controllers/**: MVC Controllers for API endpoints
  - **Models/**: Data models
  - **Services/**: Business logic and content management
  - **wwwroot/**: Static assets (CSS, JS, images)

- **Content/**: Contains all markdown files organized by type
  - **Tips/**: Individual tip markdown files
  - **Tutorials/**: Step-by-step guide markdown files
  - **Tools/**: AI tool profile markdown files
  - **Guides/**: Best practice and setup guide markdown files
  - **News/**: Latest updates and announcements

## Philadelphia/Regional Flavor

As the name suggests, "Copilot That Jawn" incorporates Philadelphia terminology ("jawn") and regional flavor. The site includes Philadelphia-themed examples, case studies, and local tech community resources, with design elements that reference Philadelphia's cultural identity.

## Technology Stack

- **Framework**: ASP.NET Core 9.0+ with .NET Aspire cloud-native stack
- **Service Orchestration**: .NET Aspire AppHost for managing Redis, Azure Storage, and web application
- **Caching**: Redis distributed caching with multi-layer caching strategy
- **Content Storage**: Markdown files in Content/ directory processed with Markdig
- **Frontend**: Bootstrap 5, modern responsive design with CSS Grid/Flexbox
- **JavaScript**: Vanilla JS with modern ES6+ features, minimal dependencies
- **Observability**: Built-in logging, metrics, and distributed tracing through Aspire

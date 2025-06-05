# Technical Architecture

## Technology Stack

"Copilot That Jawn" is built using modern web technologies with a focus on performance, maintainability, and scalability:

- **Backend Framework**: ASP.NET Core 9.0+
- **Frontend**: Bootstrap 5 with custom CSS using Grid/Flexbox
- **Content Processing**: Markdown (using Markdig library) with YAML frontmatter (using YamlDotNet)
- **JavaScript**: Vanilla JS with modern ES6+ features
- **Build/Development**: .NET CLI with hot reload via `dotnet watch`

## Application Architecture

The application follows a clean architecture pattern with separation of concerns:

### Web Layer

- **Razor Pages**: Used for content-focused pages with minimal logic
- **MVC Controllers**: Used for API endpoints and more complex interactions
- **View Components**: Reusable UI components across pages

### Business Layer

- **Services**: Business logic, content processing, and external integrations
- **Repositories**: Data access patterns for any database interactions

### Data Layer

- **Content Files**: Markdown files with YAML frontmatter for structured content
- **Database** (if implemented): Entity Framework Core with SQL Server or SQLite

## Content Management

The content management approach prioritizes developer-friendly workflows:

1. **Markdown Files**: All content is stored as Markdown files in the `Content/` directory
2. **YAML Frontmatter**: Each file contains metadata like title, tags, categories, and publication date
3. **Content Service**: The `ContentService` class processes these files and provides them to the application
4. **Caching**: Content is cached for performance while allowing for easy updates

## Site Generation

The site combines dynamic content processing with static asset optimization:

1. **Content Processing**: Markdown files are parsed at runtime by the ContentService
2. **Dynamic Routes**: Content is served through dynamic routes like `/tips/{slug}`
3. **SEO Features**: Sitemap.xml generation, meta tags, and OpenGraph support
4. **Performance Optimization**: Response compression, caching strategies, and optimized assets

## Deployment and Hosting

The application is designed to be deployed to:

- **Azure App Service**: Primary hosting option
- **Docker Containers**: Containerization support
- **Local Development**: Full support for local development with hot reload

## Diagrams

### Application Flow

```
User Request → ASP.NET Core Pipeline → Razor Pages/Controllers → ContentService → Markdown Processing → HTML Response
```

### Content Processing Flow

```
Markdown Files → YAML Frontmatter Parsing → Markdown Processing → HTML Generation → View Rendering → User Response
```

### Component Relationships

```
Razor Pages ↔ ContentService ↔ Markdown Files
Controllers ↔ ContentService ↔ Markdown Files
```

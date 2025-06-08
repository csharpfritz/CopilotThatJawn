# Copilot That Jawn - Development Instructions

## Project Overview
This is "Copilot That Jawn" - a comprehensive ASP.NET Core web application that showcases a curated collection of AI tools, tips, tricks, and resources with a primary focus on the Microsoft Copilot ecosystem. The website serves as a hub for developers, content creators, and AI enthusiasts to discover and learn about various AI-powered tools and best practices.

## Project Purpose
- **Primary Goal**: Create an educational and resource-rich platform for Microsoft Copilot and AI tools
- **Target Audience**: Developers, content creators, students, and professionals interested in AI productivity tools
- **Content Focus**: Microsoft Copilot (GitHub Copilot, Microsoft 365 Copilot, Copilot Studio), Azure AI services, and complementary AI tools

## Technology Stack
- **Framework**: ASP.NET Core 9.0+ with Razor Pages
- **Architecture**: Razor Pages application with MVC controllers for complex operations
- **Content Storage**: Markdown files in Content/ directory for tips, tutorials, and guides
- **Database**: Entity Framework Core (SQL Server or SQLite for development) for dynamic data
- **Frontend**: Bootstrap 5, modern responsive design with CSS Grid/Flexbox
- **JavaScript**: Vanilla JS with modern ES6+ features, minimal dependencies

## Project Structure Guidelines

### Web/ Directory Structure
```
Web/
├── Controllers/          # MVC Controllers for complex logic and API endpoints
├── Pages/               # Razor Pages for main website pages and content display
├── Models/              # Data models and ViewModels
├── Data/                # Entity Framework context and migrations
├── Services/            # Business logic and external API integrations
├── Components/          # Razor Components and ViewComponents
├── wwwroot/            # Static assets (CSS, JS, images)
└── Areas/              # Feature-based organization (Admin, API, etc.)
```

### Content/ Directory Structure
```
Content/
├── Tips/               # Individual tip markdown files
├── Tutorials/          # Step-by-step guide markdown files
├── Tools/              # AI tool profile markdown files
├── Guides/             # Best practice and setup guide markdown files
└── News/               # Latest updates and announcements
```

**Content Organization**: All tips, tutorials, and guides are stored as markdown files in the `Content/` directory. This allows for easy content management, version control, and collaboration. Each markdown file contains frontmatter metadata (title, description, tags, category, etc.) and the main content body.

## Content Categories & Features

### Main Content Areas
1. **AI Tools Showcase**
   - GitHub Copilot features and extensions
   - Microsoft 365 Copilot capabilities
   - Azure AI services and tools
   - Third-party AI productivity tools
   - Tool comparisons and reviews

2. **Tips & Tricks**
   - Prompt engineering best practices
   - Workflow optimization techniques
   - Integration strategies
   - Productivity hacks
   - Code snippets and examples

3. **Tutorials & Guides**
   - Step-by-step setup guides
   - Best practice documentation
   - Video tutorials and demos
   - Case studies and use cases

4. **Community Features**
   - User-submitted tips
   - Tool ratings and reviews
   - Discussion forums or comments
   - Newsletter signup
   - Social sharing integration

## Coding Standards & Best Practices

### ASP.NET Core Specific
- Use dependency injection for all services
- Implement proper error handling and logging
- Follow RESTful conventions for APIs
- Use async/await patterns for I/O operations
- Implement proper model validation
- Use strongly-typed views and models

### Data Models
- Create separate models for database entities, DTOs, and ViewModels
- Use Data Annotations for validation
- Implement IValidatableObject for complex validation
- Follow naming conventions (PascalCase for properties)

### Frontend Guidelines
- Mobile-first responsive design
- Accessibility compliance (WCAG 2.1 AA)
- Progressive enhancement
- Semantic HTML5 structure
- Modern CSS with CSS Grid and Flexbox
- Minimal JavaScript, prefer vanilla JS over heavy frameworks

### Code Formatting Guidelines
- HTML block-level elements must start on a new line
  ```html
  <div>
    <p>
      Content here
    </p>
    <section>
      <h2>Heading</h2>
      <p>More content</p>
    </section>
  </div>
  ```
- C# statements must start on a new line, including after control flow keywords
  ```csharp
  if (condition)
  {
      DoSomething();
      AnotherStatement();
  }
  
  foreach (var item in items)
  {
      ProcessItem(item);
  }
  ```

### Database Design
- Use Entity Framework Core with Code First approach
- Implement proper indexing for search functionality
- Include audit fields (CreatedDate, ModifiedDate, CreatedBy)
- Use appropriate data types and constraints
- Plan for content categorization and tagging

## Key Features to Implement

### Core Functionality
1. **Content Management**
   - CRUD operations for tools, tips, and tutorials
   - Category and tag management
   - Search and filtering capabilities
   - Content rating and review system

2. **User Experience**
   - Fast loading times with optimized images
   - Intuitive navigation and breadcrumbs
   - Advanced search with filters
   - Bookmarking and favorites
   - Social sharing buttons

3. **Performance & SEO**
   - Server-side rendering for SEO
   - Meta tags and Open Graph support
   - XML sitemap generation
   - Schema.org markup for rich snippets
   - Image optimization and lazy loading

### Technical Features
1. **APIs and Integrations**
   - GitHub API integration for Copilot-related repositories
   - Microsoft Graph API for Office 365 content
   - RSS feed generation
   - Newsletter integration (Mailchimp, SendGrid)

2. **Admin Features**
   - Content management dashboard
   - Analytics and reporting
   - User management (if applicable)
   - Bulk content operations

## Philadelphia/Regional Flavor
- Incorporate "jawn" terminology appropriately in UI text
- Use Philadelphia-themed examples and case studies
- Include local tech community resources
- Consider Philly color schemes (eagles green, phillies red, etc.)
- In content, don't refer to the Philly Dev Community, just refer to the 'Tech Community'

## Development Workflow

### Naming Conventions
- **Controllers**: `{Feature}Controller.cs` (e.g., `ToolsController.cs`)
- **Models**: `{Entity}Model.cs`, `{Entity}ViewModel.cs`, `{Entity}Dto.cs`
- **Services**: `I{Service}Service.cs` and `{Service}Service.cs`
- **Pages**: Descriptive names matching URL structure
- **CSS Classes**: Use BEM methodology (block__element--modifier)

### Code Organization
- Group related functionality in the same namespace/folder
- Use partial classes for large models if needed
- Implement repository pattern for data access if complex
- Separate concerns clearly (presentation, business logic, data)

### Testing Strategy
- Unit tests for business logic and services
- Integration tests for controllers and data access
- End-to-end tests for critical user workflows
- Performance testing for content-heavy pages

### Development Workflow Best Practices

#### Running and Testing the Application
- **Prefer `dotnet watch`**: Always use `dotnet watch run` for development instead of manually building, running, stopping, and restarting the application
- **Hot Reload Benefits**: `dotnet watch` provides automatic rebuilding and browser refresh when files change, significantly improving development efficiency
- **Recommended Commands**:
  - `dotnet watch run` - Start the application with hot reload
  - `dotnet watch test` - Run tests with automatic re-execution on file changes
  - `dotnet watch build` - Build with automatic rebuilding on changes

#### File Monitoring
- `dotnet watch` automatically monitors C# files, Razor pages, CSS, JavaScript, and other static assets
- Changes trigger automatic compilation and browser refresh
- Reduces context switching and speeds up the development cycle

#### Development Environment Setup
- Ensure the development environment is configured for optimal `dotnet watch` performance
- Use the `--verbose` flag for debugging watch issues: `dotnet watch run --verbose`
- Configure file exclusions in `.csproj` if needed to avoid unnecessary rebuilds

## Content Strategy

### Content Types
- **Tool Profiles**: Detailed pages for each AI tool with features, pricing, pros/cons
- **Tip Cards**: Bite-sized productivity tips with examples
- **Tutorial Series**: Multi-part guides for complex topics
- **News & Updates**: Latest developments in the Copilot ecosystem
- **Community Contributions**: User-submitted content and reviews

### SEO Considerations
- Target keywords: "GitHub Copilot", "Microsoft Copilot", "AI coding tools", "productivity tips"
- Create comprehensive, valuable content that ranks well
- Implement proper internal linking structure
- Optimize for featured snippets and voice search

## Security & Performance

### Security Measures
- Input validation and sanitization
- CSRF protection
- Content Security Policy headers
- Rate limiting for APIs
- Secure authentication if user features are added

### Performance Optimization
- Enable response compression
- Implement caching strategies (memory, distributed)
- Optimize database queries
- Use CDN for static assets
- Implement lazy loading for images and content

## Future Enhancements
- AI-powered content recommendations
- Interactive demos and playgrounds
- Integration with learning management systems
- Mobile app companion
- API for third-party integrations

## Development Notes
- Prioritize user experience and content quality
- Keep the design clean and professional
- Ensure fast loading times across all devices
- Make content easily discoverable and shareable
- Plan for scalability as content grows

Remember: This is a showcase of AI tools and productivity, so the site itself should demonstrate best practices in modern web development and user experience design.
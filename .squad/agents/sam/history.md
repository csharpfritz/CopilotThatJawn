# Sam — History

## Core Context
- Lead / Content Strategist for Copilot That Jawn
- Project: ASP.NET Core 9.0+ website showcasing AI tools, tips, and resources
- Content stored as markdown files in Content/ directory with YAML frontmatter
- Content categories: GitHub Copilot, Microsoft 365 Copilot, Azure AI, Database Development, Project Management, Marketing & Communications
- User: Jeffrey T. Fritz

## Learnings

### Editorial Content Calendar Strategy (12-Week MWF Plan, Feb 23, 2026)

**Calendar Created**: March 2 – May 22, 2026 (36 articles, 3/week MWF schedule)

**Strategic Decisions**:
1. **Gap Coverage Prioritization**: All 11 high-priority gaps front-loaded into Weeks 1–4 (12 articles) for immediate audience impact and SEO authority. Medium-priority (14 gaps) covered by Week 10; lower-priority (7 gaps) by Week 12.

2. **Series Architecture** (5 thematic clusters):
   - **Copilot Across IDEs** (4 parts): JetBrains → Xcode → Neovim → VS 2026. Natural progression for cross-platform developer workflows. Completed by Week 4.
   - **Copilot for Teams** (2 parts): Enterprise governance + agent mode workflows. Bridges individual to org scale. Weeks 5–6.
   - **Enterprise AI & Governance** (2 parts): CI/CD automation + MCP custom integrations. Advanced power users. Weeks 7–8.
   - **Advanced Integrations** (3 parts): Slack/Communication → Cloud MCPs → Pipeline skills. Infrastructure/DevOps focus. Weeks 9–10.
   - **M365 Collaboration** (2 parts): Loop co-authoring + custom vertical agents. Teams & business users. Weeks 11–12.

3. **Difficulty Distribution** (audience accessibility):
   - ~36% Beginner (13 articles): Establishes foundational knowledge; broadens addressable audience.
   - ~50% Intermediate (18 articles): Core content for users scaling beyond basics; heaviest engagement tier.
   - ~14% Advanced (5 articles): Power users, specialists; niche but high-authority content.
   - **No 3+ consecutive Advanced articles** to prevent audience drop-off.

4. **Category Rotation** (avoid fatigue):
   - **GitHub Copilot** (47%, 17 articles): IDE-focused, plus extensions/MCPs/enterprise topics.
   - **Microsoft 365 Copilot** (22%, 8 articles): Outlook, Teams, document collaboration, thinking modes.
   - **Azure AI** (19%, 7 articles): Copilot Studio, Azure AI Foundry, industry-specific solutions.
   - **Database & Cross-Cutting** (12%, 4 articles): Specialized verticals, foundational topics.
   - **Pattern**: Avoids single-category saturation; diversifies audience engagement weekly.

5. **Flex Slots** (2 reserved):
   - Week 7 Friday (April 17) & Week 11 Wednesday (May 13).
   - Ensures breaking news, product releases, community requests don't derail editorial calendar.
   - Can be pre-staged as "draft" for rapid publication.

**Sequencing Rationale**:
- **Weeks 1–2**: High-impact IDEs + foundational M365 (Vision, Outlook, Thinking Modes). Captures early adopters and establishes credibility.
- **Weeks 3–4**: Complete IDE series + org-scale features (Workspace, Spaces, governance). Transitions readers from individual to team context.
- **Weeks 5–6**: Enterprise/org topics (governance, agent mode, enterprise plan). Mid-season shift to decision-makers and IT teams.
- **Weeks 7–8**: Advanced CI/CD + custom integrations (MCPs, extensions). Attracts platform builders and DevOps specialists.
- **Weeks 9–10**: Niche integrations + lower-priority gaps (Slack, Cloud MCPs, industry-specific AI). Consolidates power-user and specialist segments.
- **Weeks 11–12**: M365 deep collaboration + future-focused topics (Loop, custom copilots, production deployment). Wraps with team/org collaboration themes.

**Content Handoff Notes**:
- **Diane (Writer)**: Prioritize high-priority articles (Weeks 1–4) with visual guides; coordinate with platform docs for accuracy.
- **Cliff (Researcher)**: Deliver gap research for Weeks 1–4 by mid-February; Azure AI Foundry requires deep hands-on exploration.
- **Carla (Publishing)**: Use series metadata (`series: "Copilot Across IDEs"`, `part: 1`) for cross-linking; mark Weeks 1 articles as `featured: true`.
- **Norm & Scribe**: Monitor product release cycles (GitHub, Microsoft, Azure) for timing alignment; pre-stage flex slot content as drafts.

**Success Metrics**:
1. Publish 34–36 articles on MWF schedule (allowing flex slots to be used strategically).
2. High-priority articles (Weeks 1–4) achieve 2x average engagement (audience ramp-up effect).
3. Beginner articles maintain consistent engagement (validates accessibility approach).
4. Series articles generate 20%+ internal click-through rates (cross-series discoverability).

**Cross-Agent Learnings (Feb 23, 2026):**
- Cliff's 32-gap analysis confirms opportunity for comprehensive content coverage; calendar systematically addresses all gaps
- Confirmed feasibility: website infrastructure supports MWF publishing cadence without modifications
- Series clustering strategy drives cross-linking and SEO; recommend consistent use of series/part metadata fields

### Website Architecture & Technology Stack
- **Web Framework**: ASP.NET Core 9.0+ with Razor Pages and MVC Controllers
- **Architecture**: .NET Aspire-based microservices orchestration (cloud-native)
- **AppHost Configuration**: Located in AppHost/AppHost.cs — orchestrates Web, Redis, and Azure Storage (Emulator locally)
- **Caching Strategy**: Three-tier caching system:
  1. Memory cache (local, 5-min expiry) for frequently accessed tips
  2. Redis distributed cache (6-hour expiry) for cross-server consistency
  3. Output cache store for page-level caching
- **Storage**: Azure Storage (Tables + Blobs locally via emulator, production in cloud)
  - Table Storage: Stores tip metadata and content
  - Blob Storage: Stores article images and associated media

### Content Pipeline (Markdown → Website)

**How it works:**
1. Markdown files with YAML frontmatter stored in `Content/Tips/` directory
2. ContentLoader (standalone .NET service) processes markdown files:
   - Parses YAML frontmatter using YamlDotNet
   - Extracts metadata (title, category, tags, difficulty, author, dates)
   - Processes images from `Content/Tips/images/` subdirectory
   - Uploads images to Azure Blob Storage with CDN-friendly image IDs
   - Relinks image references in markdown to cloud URLs
   - Uploads tip to Azure Table Storage as content entity
3. Web app ContentService reads from Table Storage on-demand:
   - Caches with Redis for performance
   - Renders markdown to HTML using Markdig pipeline
   - Supports filtering by category, tags, difficulty, search terms

**Frontmatter Fields Required:**
- `title` (string)
- `description` (short summary)
- `category` (main topic area)
- `tags` (array of keywords)
- `difficulty` (Beginner/Intermediate/Advanced)
- `author` (author or team name)
- `publishedDate` (YYYY-MM-DD)
- `lastModified` (YYYY-MM-DD, optional but recommended)

### Current Content State
**31 published tips** across 6 categories:
1. **GitHub Copilot & AI Coding** (7 tips): Instructions, MCP integration, Skills, Context7, Agent mode, unit tests
2. **Microsoft 365 Copilot** (10 tips): Word, PowerPoint, Excel, finance, teams, summaries, presentations
3. **Azure & Cloud AI** (1 tip): Azure Copilot basics
4. **Project Management & Productivity** (3 tips): Project management, user manuals, automation
5. **Marketing & Communications** (1 tip): Marketing playbook
6. **Database Development** (1 tip): Database development guide
7. **Additional Tips** (8 tips): Chat commands, career consulting, checklist interviewer, legacy code refactoring, etc.

**Images**: 15 images across marketing playbook, instructions, team meetings, and summarization guides

### Website Structure
**Pages:**
- Home (`Pages/Index.cshtml`) — Hero section with feature highlights and CTAs
- Tips Index (`Pages/Tips/Index.cshtml`) — Grid layout with filtering, search, pagination (3-column on large screens)
- Tip Details (`Pages/Tips/Details.cshtml`) — Full article view with navigation (previous/next tips)
- Category View (`Pages/Tips/Category.cshtml`) — Tips filtered by category
- Tag View (`Pages/Tips/Tag.cshtml`) — Tips filtered by tag
- About, Privacy, Terms, Contribute pages

**Navigation:**
- Top navbar with brand logo, main nav links
- Footer with links and social media
- Dark/light theme toggle via localStorage
- Responsive design using Bootstrap 5

**Key Features:**
- SEO optimized: meta tags, Open Graph, Twitter cards, canonical links
- PWA manifest and favicons configured
- Reading time estimation (Content.Length / 200 words)
- Difficulty badges with color coding
- Related tips navigation
- Tag and category clouds

### Development & Build Process
- **Primary Dev Command**: `dotnet run --project AppHost` (starts entire Aspire stack)
- **With Hot Reload**: `dotnet watch run --project AppHost`
- **Redis, Azure Storage Emulator**: Configured in AppHost, start automatically
- **Asset Bundling**: WebOptimizer configured for CSS/JS minification (production) and development
- **Running Tests**: `dotnet test` in individual project directories

### Uncommitted Changes & New Content
**Modified files:**
- `.github/tips-summary.md` — Updated but not committed

**Untracked files (in .squad-templates):**
- GitHub workflows for CI/CD (squad-*, sync operations)
- Squad agent configuration files

### Key File Locations for Editorial
- **Content source**: `Content/Tips/*.md` (each with frontmatter)
- **Images**: `Content/Tips/images/` (organized by tip)
- **Tips index**: `.github/tips-summary.md` (community reference, maintained)
- **Content models**: `Shared/TipModel.cs` (defines metadata structure)
- **Content service**: `Web/Services/ContentService.cs` (parsing, caching, filtering)
- **Upload helper**: `Shared/ContentUploadHelper.cs` (YAML parsing, image processing)
- **ContentLoader**: `ContentLoader/Program.cs` (batch markdown → storage sync)

## Calendar Revision — Week 1 & Series Architecture Update (Task from Jeffrey)

**Added**: "Create Your First Custom Copilot Agent: Authoring .agent.md Files for Specialized AI Assistants"
- **Placement**: Week 1 Wednesday, March 4, 2026 (Beginner, GitHub Copilot)
- **Rationale**: Foundational article addressing gap in .github/agents/*.agent.md content. Pairs naturally with existing copilot-instructions.md and SKILL.md articles. Beginner-level intro helps readers understand agent customization before intermediate/advanced topics.
- **Strategic positioning**: Front-loaded in Week 1 to establish credibility in agent/customization space early; removes friction for readers discovering custom agent workflows.

**Displaced**: "GitHub Copilot Vision: From Wireframes to Code" shifted from Week 1 Wed to Week 7 Fri (April 17)
- Vision is Intermediate (less foundational); .agent.md is Beginner and more strategic to lead customization series.

**New Series**: "Copilot Customization" (3 parts)
- Part 1: Create Your First Custom Copilot Agent (.agent.md) — Week 1 Wed
- Part 2: Copilot Instructions (existing content, reference) — existing archive
- Part 3: GitHub Copilot Skills (existing content, reference) — existing archive
- Provides readers a cohesive learning path for understanding how to build tailored Copilot workflows.

**Calendar impact**: 
- Beginner articles: 13 → 14 (39% of 36 total)
- GitHub Copilot articles: 17 → 18 (50% of 36 total)
- All balance metrics remain healthy; no consecutive advanced articles; category rotation preserved.

### Session 3: Calendar Revision Finalization & Decision Approval (Feb 23, 2026)

**Scribe Integration** — Formalized calendar revision as team decision:
- Orchestration logs created for both Sam and Carla (2026-02-23T1622)
- Session log written documenting changes and team coordination
- Decision merged into `.squad/decisions.md` (canonical record)
- Inbox files archived (sam-content-calendar-UPDATED.md, carla-publishing-schedule.md)

**Cross-Agent Coordination**:
- Sam's calendar revision now integrated with Carla's publishing schedule (filenames, tags, metadata)
- Both agents' history.md updated with session context
- Team-wide handoff documented (Diane, Cliff, Norm, Scribe)

**Validation Complete**:
- All 36 articles verified as MWF dates
- No violations of sequencing principles (beginner → intermediate → advanced, category rotation, series clustering)
- High-priority gaps: 12/11 (100% + 1 foundational) by Week 4
- Medium-priority gaps: 14/14 (100%) by Week 10
- Lower-priority gaps: 7/7 (100%) by Week 12
- 1 flex slot remaining (Week 11 Wed, May 13, 2026) for breaking news/community requests

**Readiness**: Calendar now production-ready for team handoff to writers, researchers, and publishers

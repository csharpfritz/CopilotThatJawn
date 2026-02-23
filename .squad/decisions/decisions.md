# Decisions — Copilot That Jawn
**Last Updated**: February 23, 2026

---

## Content Audit & Gap Analysis (Feb 23, 2026)

### Decision: 32-Gap Content Strategy

**Context**: Comprehensive audit of existing 31 tips identified significant gaps in current content coverage relative to February 2026 feature set across GitHub Copilot, Microsoft 365 Copilot, and Azure AI.

**Decision**: Implement systematic gap-filling strategy via 12-week editorial calendar addressing all 32 gaps (11 high-priority, 14 medium-priority, 7 lower-priority).

**High-Priority Gaps** (Weeks 1–4, 12 articles):
- GitHub Copilot in Outlook (email, scheduling, action items)
- Code Review PR Comments (team workflows, automation)
- JetBrains IDE Support (IntelliJ, PyCharm, WebStorm)
- Copilot for Xcode (Swift/iOS development)
- Copilot Workspace (agentic planning/coding/review)
- Copilot in Neovim (terminal-driven development)
- GitHub Copilot Vision (wireframes to code, Figma integration)
- Copilot Spaces & Knowledge Sharing (org contexts)
- Microsoft 365 Copilot Advanced Teams (interactive agents, content analysis)
- Copilot Studio (low-code agent builder)
- Azure AI Foundry (multi-agent orchestration, RAG, fine-tuning)
- GitHub Copilot in Visual Studio 2026 (cloud agents, native integration)

**Medium-Priority Gaps** (Weeks 5–10, 14 articles):
- Copilot Extensions & Marketplace
- Custom MCP Servers
- Copilot Enterprise Plan & Governance (SAML SSO, audit logs, IP indemnity)
- Multi-Model Support (GPT-4.1, GPT-5, Codex variants)
- Thinking Modes (Quick vs Think Deeper, M365)
- CLI Agentic Workflows (advanced Plan agent usage)
- Agent Mode Deep Dive (error recovery, iterative execution)
- Next Edit Suggestions (NES) — predictive coding
- Database MCPs & SQL Integration
- CI/CD & Deployment Automation with Copilot
- Multi-Document Comparison & Analysis (M365)
- PDF & Complex Document Handling (M365)
- Copilot for Visual Studio 2026 (advanced features)

**Lower-Priority Gaps** (Weeks 11–12, 7 articles):
- Loop & M365 Collaboration
- Slack/Communication MCP Integration
- Cloud MCPs (AWS, GCP, Azure)
- Industry-Specific AI Solutions (healthcare, finance, legal)
- RAG Patterns for Custom Knowledge
- Agent Skills for CI/CD Pipelines
- Custom Copilot for Internal Tools

**Rationale**: 
- High-priority gaps represent core productivity features with broad audience appeal and search demand
- Medium-priority gaps serve specialized segments (developers, architects, power users)
- Lower-priority gaps address niche use cases and advanced patterns
- Staggered timeline allows research, writing, review, and publishing without team overload
- Early publication of high-priority content establishes authority and captures early adopters

**Outcome**: 36-article plan across 12 weeks (MWF) covers 32 gaps by May 22, 2026.

---

### Decision: Editorial Calendar Structure

**Context**: 31 existing tips well-organized but need strategic sequencing for maximum audience impact and engagement.

**Decision**: 12-week MWF calendar (March 2 – May 22, 2026) with 5 thematic series, beginner-first progression, and category rotation.

**Calendar Architecture**:

1. **Thematic Series** (cross-linking, discoverability):
   - **Copilot Across IDEs** (4 parts): JetBrains (Week 1) → Xcode (Week 3) → Neovim (Week 3) → VS 2026 (Week 4)
   - **Copilot for Teams** (2 parts): Enterprise governance (Week 5) → Agent mode workflows (Week 6)
   - **Enterprise AI & Governance** (2 parts): CI/CD automation (Week 7) → MCP custom integrations (Week 8)
   - **Advanced Integrations** (3 parts): Slack/Communications (Week 9) → Cloud MCPs (Week 9) → Pipeline skills (Week 10)
   - **M365 Collaboration** (2 parts): Loop co-authoring (Week 11) → Custom vertical agents (Week 12)

2. **Difficulty Distribution** (accessibility-first):
   - **Beginner** (~13 articles, 36%): Weeks 1–2 (IDEs, Vision, Outlook), scattered throughout for breadth
   - **Intermediate** (~18 articles, 50%): Core audience content spread across all weeks
   - **Advanced** (~5 articles, 14%): Weeks 7–12, clustered to avoid 3+ consecutive advanced articles causing audience drop-off

3. **Category Rotation** (prevent fatigue):
   - **GitHub Copilot** (17 articles, 47%): IDEs, Vision, Workspace, PR code review, Agent mode, Extensions, MCPs, Skills, CLI
   - **Microsoft 365 Copilot** (8 articles, 22%): Outlook, Teams, Thinking modes, document comparison, Loop, PDFs
   - **Azure AI** (7 articles, 19%): Copilot Studio, Azure AI Foundry, industry solutions, RAG, deployment
   - **Other** (4 articles, 12%): Database development, context7, metrics API, cross-cutting topics

4. **Flex Slots** (2 reserved):
   - Week 7 Friday (April 17)
   - Week 11 Wednesday (May 13)
   - Allows breaking news, product releases, community requests without derailing schedule

**Sequencing Rationale**:
- Weeks 1–2: High-impact topics + foundational M365 = establish credibility early
- Weeks 3–4: IDE series completion + org-scale features = transition readers from individual to team context
- Weeks 5–6: Enterprise features = capture IT/security teams
- Weeks 7–8: Advanced integrations = attract platform builders
- Weeks 9–10: Niche topics = consolidate power-user segments
- Weeks 11–12: M365 collaboration + future topics = wrap with team/org themes

**Success Metrics**:
- 34–36 articles published on MWF schedule
- High-priority articles (Weeks 1–4) achieve 2x average engagement
- Beginner articles maintain consistent engagement across all weeks
- Series articles generate 20%+ internal cross-linking click-through

---

### Decision: Website Architecture & Content Pipeline Validation

**Context**: Confirmed ASP.NET Core 9.0+ Aspire-based website with mature content pipeline.

**Decision**: Existing infrastructure supports planned publishing cadence without modifications. Content submission process verified and documented.

**Key Findings**:
- **Framework**: ASP.NET Core 9.0+ with Razor Pages, MVC Controllers, .NET Aspire orchestration
- **Caching**: Three-tier system (memory 5-min, Redis 6-hour, output cache) ensures performance
- **Storage**: Azure Table Storage (metadata) + Blob Storage (images), locally emulated
- **Pipeline**: Markdown (YAML frontmatter) → ContentLoader CLI → Table Storage → cached web rendering
- **Capacity**: 31 existing tips confirmed; pipeline tested; 36-article plan feasible

**Editorial Workflow** (verified):
1. Create `.md` file in `Content/Tips/` with YAML frontmatter (title, description, category, tags, difficulty, author, dates)
2. Place images in `Content/Tips/images/` subdirectory
3. Run: `dotnet run --project ContentLoader -- Content/Tips`
4. Content appears on site within cache refresh window (~6 hours via Redis)
5. No manual YAML parsing or image linking required; ContentLoader handles rewriting

**Frontmatter Fields** (required):
- `title`, `description`, `category`, `tags`, `difficulty`, `author`, `publishedDate`, `lastModified`

**Content Pipeline Strengths**:
- Fully automated markdown-to-HTML rendering
- Image processing and CDN URL rewriting
- Category, tag, and difficulty filtering
- Reading time estimation
- SEO optimization (meta, Open Graph, Twitter cards)

**No Documentation Gap**: Contributor guide should be created (noted for future session).

---

### Decision: Content Gap Inventory

**Existing Strengths**:
- MCP ecosystem: Well-covered with practical examples (GitHub, Microsoft Docs, Context7, Playwright, Google Analytics)
- Microsoft 365 Excel: Comprehensive coverage including specialized finance and attendance workflows
- Agent Skills: Good beginner-friendly introduction with real-world example
- Custom Instructions: Thorough coverage of project-level guidance and prompt files
- Documentation Automation: Practical guide with MCP integration

**Identified Weaknesses**:
- **IDE Support**: Only VS Code/Visual Studio implied; missing JetBrains (very popular), Xcode, Neovim
- **Outlook Integration**: Completely missing; major 2026 feature for email/calendar workflows
- **Code Review**: No comprehensive guide despite being a key feature
- **Enterprise Features**: Copilot Spaces, Enterprise plan, governance, security policies not covered
- **Advanced M365**: Thinking modes, interactive agents, content analysis barely mentioned
- **Azure AI Services**: Copilot Studio and Azure AI Foundry completely absent (strategic gap)
- **Advanced Development**: Cloud MCPs, CI/CD automation, custom extensions underexplored

**Opportunities**:
- "Copilot Across IDEs" series addresses IDE gap comprehensively
- "Copilot in Outlook" guide captures 2026 feature adoption
- "Copilot Code Review Workflow" serves team leads and engineering managers
- Expanded MCP coverage (database, cloud, CI/CD) increases power-user engagement
- Azure AI Foundry & Copilot Studio foundation courses establish thought leadership

---

## Cross-Agent Learnings

**From Cliff (Researcher)**:
- Knowledge Bases sunset Nov 1, 2025 → Copilot Spaces is successor; content strategy must reflect this
- Visual Studio 2026 + cloud agents (late 2025) and Teams interactive agents (Feb 2026) are major features; priority coverage recommended
- MCP ecosystem is rapidly expanding; custom MCP server building is growing developer demand

**From Sam (Lead)**:
- Content pipeline is battle-tested and performant; no infrastructure barriers to publishing scale
- Series clustering with cross-linking metadata drives discoverability; recommend consistent use of `series` and `part` frontmatter fields
- Audience arc (Beginner → Intermediate → Advanced) prevents reader fatigue and maximizes SEO authority over time

---

## References & Source Documents

- `.squad/decisions/inbox/cliff-content-gap-analysis.md` — Full feature research and gap analysis
- `.squad/decisions/inbox/sam-site-review.md` — Website architecture and content pipeline validation
- `.squad/decisions/inbox/sam-content-calendar.md` — Complete 12-week editorial calendar with implementation notes
- `.squad/agents/cliff/history.md` — Researcher context and learnings
- `.squad/agents/sam/history.md` — Content strategist context, architecture notes, and learnings

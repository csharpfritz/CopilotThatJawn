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

## Week 1 Article Production & Quality Gate (March 2–7, 2026)

### Decision: Content Branch & PR Publishing Workflow

**Context:** User directive captured February 23, 2026: Articles should be queued in separate feature branches with separate PRs, with each PR declaring its publish schedule for scheduled releases.

**Decision:** Adopt feature-branch + draft-PR model for all article submissions. Each article gets a dedicated branch, draft PR with publish date in frontmatter (publishedDate field + featured flag), and quality gate before merge.

**Implementation:**
- Article 1 (JetBrains): Branch `content/2026-03-02-jetbrains-ide-copilot` → PR #29 (draft) → Publish Monday March 2
- Article 2 (Custom Agent): Branch `content/2026-03-04-custom-copilot-agent` → PR #30 (draft) → Publish Wednesday March 4
- Article 3 (Outlook): Branch `content/2026-03-06-outlook-copilot` → PR #28 (draft) → Publish Friday March 6

**Rationale:**
- Separate branches allow parallel writing & research
- Draft PRs enable pre-publish quality review (Norm's gate)
- Frontmatter dates + featured flag declare schedule clearly
- Enables scheduled merges aligned with MWF publishing calendar
- Reduces merge conflicts and release coordination overhead

**Outcome:** All 3 Week 1 articles submitted as draft PRs with publish schedule declared. Ready for Norm's quality gate and Carla's publication.

---

### Decision: Microsoft 365 "Getting Started" Article Template

**Context:** Diane authored "Microsoft Copilot in Outlook" (Week 1 article) and identified successful pattern for M365 feature guides.

**Decision:** Establish reusable template for Microsoft 365 Copilot features (Outlook, Teams, Word, Excel, PowerPoint):
1. **Problem opener** (e.g., "buried under emails") establishes relatable pain point
2. **"What Copilot Can Do"** feature overview (beginner focus, highest ROI features first)
3. **Setup/Prerequisites** including licensing clarity upfront (base subscription vs. add-on)
4. **Three practical, end-to-end examples** demonstrating key features
5. **Pro Tips** for maximizing productivity
6. **Tone**: "Executive assistant" framing, business outcomes focus, time-saving language

**Rationale:**
- M365 audience skews business professionals, not software developers
- Licensing transparency prevents roadblocks (IT admins, budget holders read these)
- Practical examples build reader investment
- Template mirrors successful Excel/Word pattern; reduces writing time for future M365 articles
- "Executive assistant" framing resonates with productivity messaging

**Outcome:** Template established and validated with Outlook article (1,350 words, draft PR #28). Applies to Teams, Word, Excel, PowerPoint, and other M365 Copilot features.

---

### Decision: Copilot Across IDEs Series Launch (JetBrains as Part 1)

**Context:** Diane authored "GitHub Copilot in JetBrains IDEs" (Week 1 article) and established series positioning for IDE ecosystem coverage.

**Decision:** Launch "Copilot Across IDEs" series with JetBrains as Part 1/4. Follow with VS Code, Visual Studio 2026, and Neovim in subsequent weeks.

**Series Strategy:**
- **Part 1 (Week 1, March 2):** JetBrains IDEs (IntelliJ, PyCharm, Rider, WebStorm, PhpStorm)
- **Part 2 (Week 3):** Xcode (Swift/iOS development)
- **Part 3 (Week 3):** Neovim (terminal-driven development)
- **Part 4 (Week 4):** Visual Studio 2026 (cloud agents, native integration)

**Key Findings:**
- JetBrains IDEs support full Copilot feature set: inline suggestions, Chat, Edit Mode, Agent Mode, code review, MCP integration
- Free plan availability (2,000 completions/50 chat requests/month) removes adoption barrier for individual developers
- IDE compatibility: version 2021.3+ across all major products
- Effective guides include prerequisites, step-by-step installation, feature explanations, language/IDE-specific tips, best practices, troubleshooting

**Rationale:**
- Starting with JetBrains (enterprise/mid-market presence) establishes credibility in professional environments
- Free tier emphasis drives early adoption across all IDE articles
- Series positioning enables 20%+ internal cross-linking CTR (per editorial calendar decision)
- Each part will include language/IDE-specific examples (Java/Kotlin, Python, C#/.NET, JavaScript/TypeScript, Swift, Lua/Vimscript)

**Outcome:** Part 1 complete (draft PR #29, 1,430 words). Parts 2–4 scheduled for Weeks 3–4 per editorial calendar.

---

### Decision: Copilot Customization Series & .agent.md Article Template

**Context:** Diane authored "Create Your First Custom Copilot Agent: Authoring .agent.md Files" (Week 1 article) and established content patterns for agent customization trilogy.

**Decision:** Launch "Copilot Customization" series (3 parts) with .agent.md article as Part 1. Establish template for agent/custom instruction guides.

**Series Architecture:**
- **Part 1 (Week 1, March 4):** .agent.md file authoring (personas, boundaries, commands, examples)
- **Part 2 (TBD):** Advanced agent patterns (MCP integration, error recovery, iterative execution)
- **Part 3 (TBD):** Team & organizational agent strategies

**Article Content & Structure:**
- YAML frontmatter reference (6 fields: name, description, target, tools, disable-model-invocation, metadata)
- Clear differentiation: global instructions (always-on) vs. SKILL.md (workflows) vs. .agent.md (named personas)
- 7-step hands-on walkthrough building a "code-reviewer" agent from scratch
- Real-world example: Squad agent (.github/agents/squad.agent.md) from this repo
- Comparison table: instructions vs. skills vs. agents across scope, invocation methods, use cases
- Table of 7 common agent ideas (code-reviewer, docs-writer, test-writer, prompt-optimizer, error-debugger, code-explorer, style-guide-enforcer)
- Cross-linking to copilot-instructions.md and skills article (trilogy framing)

**Key Learnings:**
- Comparison tables are essential for addressing "when do I use X vs. Y?" confusion upfront
- Real repo examples ground theory in practice and lower imposter syndrome
- Step-by-step walkthroughs mirror actual development workflow (file → config → content → test → commit)
- Emoji callouts (✨, 🎯, 🔧, 📦, ✅, 🔍, 💡) improve scannability without sacrificing professionalism
- Series metadata (series: "Copilot Customization", part: 1, featured: true) enables automatic cross-linking for 20%+ CTR goal

**Rationale:**
- Agents are natural next step after instructions/skills in customization journey
- Real-world example (Squad) proves feature is actively used
- Templates and ideas table provide immediate actionable direction for readers
- Trilogy narrative builds authority across instruction, skill, and agent topics

**Outcome:** Part 1 complete (draft PR #30, 2,100 words). Parts 2–3 scheduled per content strategy refinement.

---

## Cross-Agent Learnings

**From Diane (Writer) — Week 1:**
- **Free plan emphasis drives adoption:** Highlighting 2,000 completions/50 chat requests/month free tier removes barrier for individual developers across all IDE and tool guides
- **Real repo examples ground theory:** Referencing Squad agent in .agent.md article demonstrates feature is production-ready and lowers "can I do this?" anxiety
- **Tone shifts by audience:** GitHub Copilot articles emphasize code quality & developer productivity; Microsoft 365 articles emphasize business outcomes & time-saving
- **"Jawn" integration requires context:** Works naturally in closing statements ("Your inbox doesn't have to be a jawn that drains your day") but doesn't fit everywhere; don't force it
- **Series metadata is critical:** frontmatter (series: "Name", part: N) enables Carla's cross-linking strategy; omitting breaks discoverability goal of 20%+ CTR
- **Comparison tables solve confusion:** Side-by-side (instructions vs. skills vs. agents) answers "when do I use X?" upfront and reduces reader friction
- **Beginner articles need visual landmarks:** Emoji callouts, step-by-step breakdown, and "Pro Tips" sections keep non-expert readers engaged

**From Norm (Reviewer) — Pending:**
- Quality gate feedback on accuracy, tone consistency, frontmatter compliance (to be logged upon review completion)

---

## References & Source Documents

- `.squad/orchestration-log/2026-03-02T0800Z-diane-jetbrains-article.md` — JetBrains IDE article spawn details
- `.squad/orchestration-log/2026-03-04T0900Z-diane-agent-md-article.md` — Custom agent article spawn details
- `.squad/orchestration-log/2026-03-06T0730Z-diane-outlook-article.md` — Outlook article spawn details
- `.squad/orchestration-log/2026-03-07T1000Z-norm-quality-review.md` — Quality review initiation
- `.squad/log/2026-03-07T1130Z-week1-articles.md` — Complete session log with production details, learnings, and handoff to publishing
- `.squad/agents/diane/history.md` — Writer context and learnings
- `.squad/agents/norm/history.md` — Reviewer context and learnings

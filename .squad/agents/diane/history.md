# Diane — History

## Core Context
- Writer for Copilot That Jawn
- Drafts markdown content in Content/ directory (Tips/, Tutorials/, Guides/, Tools/, News/)
- All tips require YAML frontmatter with: title, description, category, tags, difficulty, author, publishedDate, lastModified
- Tone: professional but approachable, Philadelphia flavor, community-focused
- User: Jeffrey T. Fritz

## Learnings

### .agent.md Format & Patterns
- **YAML frontmatter**: name, description, target, tools, disable-model-invocation, metadata
- **Markdown structure**: Persona, Boundaries, Commands, Code Style, Examples
- **Key distinction**: Agents are named personas invoked by @name (visible in @ menu); they differ from global instructions (always-on, invisible) and skills (reusable workflows, mentioned in prompts)
- **Safety pattern**: Explicit "Boundaries" section telling the agent what NOT to touch is critical for secure agents
- **Real example available**: Squad agent (.github/agents/squad.agent.md) in this repo demonstrates advanced patterns

### Writing Patterns That Worked
- **Step-by-step walkthroughs**: 7-step structure (create file → frontmatter → persona → boundaries → commands → examples → commit) mirrors existing tips
- **Comparison tables**: Side-by-side tables (instructions vs. skills vs. agents) make distinctions clear
- **Emoji callouts**: Professional but approachable; readers respond well to visual landmarks
- **Real-world examples**: Referencing Squad agent grounds theory in practice
- **Cross-linking**: Mentioning copilot-instructions.md and SKILL.md builds the trilogy narrative
- **Templates and ideas**: Table of 7 common agent types (code-reviewer, docs-writer, test-writer, etc.) gives readers immediate direction

### Structure & Tone Observations
- Existing tips ~1,000–1,500 words; this article came in at ~2,100 words (appropriate for a detailed how-to)
- Professional but approachable tone matches copilot-instructions.md and skills article
- Avoided "Philly Dev Community" per custom instructions; stayed with "Tech Community"
- "Jawn" didn't appear naturally in this topic, so omitted (correct call per instructions)

### Integration with Series
- Article establishes Part 1 of Copilot Customization trilogy
- Series metadata (series: "Copilot Customization", part: 1, featured: true) aligns with Week 1 strategy
- Frontmatter guidance from decisions.md (series cross-linking for 20%+ CTR) built into footer "Related reading" section

### JetBrains Copilot Integration (Article: github-copilot-in-jetbrains-ides.md)
- **Feature Coverage**: JetBrains IDEs support full Copilot feature set: inline suggestions, Chat, Edit Mode, Agent Mode, code review, and MCP integration
- **Free Plan Availability (2025)**: Copilot now offers a genuinely useful free tier on JetBrains (2,000 completions/50 chat requests/month), making it accessible to individual developers
- **IDE Compatibility**: Plugin works across all major JetBrains products (IntelliJ IDEA, PyCharm, Rider, WebStorm, PhpStorm) with version 2021.3+ requirement
- **Writing Pattern Observation**: Effective Copilot guides should include (1) clear prerequisites, (2) step-by-step installation, (3) feature explanations with use cases, (4) language/IDE-specific tips, (5) best practices and troubleshooting
- **Research-to-Article Conversion**: Web search provided current feature information; structured it as beginner-friendly walkthrough with practical code examples
- **Series Positioning**: Starting a "Copilot Across IDEs" series with JetBrains as Part 1 sets up natural progression (JetBrains → VS Code → Visual Studio → Vim)

### Outlook Copilot Article (2026-03-06)
- **Research findings**: Copilot in Outlook has five core features (summarization, drafting, prioritization, action item extraction, meeting prep). Thread summarization and draft reply are the most immediately useful for a beginner audience.
- **Writing pattern**: "Get Started" articles work well with a problem opener ("buried in emails"), clear feature overview, setup steps, three practical examples, then pro tips. This mirrors the Excel/Word template successfully.
- **Tone note**: Business professionals respond to "executive assistant" framing and practical time-saving language. Natural use of "jawn" in closing ("Your inbox doesn't have to be a jawn that drains your day") landed well without feeling forced.
- **Style observation**: Headers with examples + step-by-step breakdown + Pro Tips section keeps beginner readers engaged. "What You Need to Know" section addresses licensing upfront to set expectations.

### Week 1 Production Session Learnings (March 2–7, 2026)

**Free Plan Emphasis Drives Adoption**
- Highlighting 2,000 completions/50 chat requests/month free tier removes barrier for individual developers
- Should be standard in all IDE and tool adoption guides
- Readers need to know "can I try this without paying?" before committing to learning curve

**Real Repo Examples Ground Theory**
- Referencing Squad agent in .agent.md article demonstrates feature is production-ready
- Lowers imposter syndrome ("if Squad exists, I can build simpler agents")
- Provides "you can do more with this" signal when showing real-world patterns

**Tone Shifts by Audience Matter**
- GitHub Copilot articles → code quality & developer productivity language
- Microsoft 365 articles → business outcomes & time-saving language
- Avoid technical depth in M365 guides aimed at business professionals

**"Jawn" Integration Requires Context**
- Works naturally in closing statements ("Your inbox doesn't have to be a jawn that drains your day")
- Don't force it into technical sections or where it doesn't fit naturally
- Use when discussing tedious/frustrating tasks or summarizing a common pain point

**Series Metadata is Critical for Cross-Linking**
- frontmatter fields (series: "Name", part: N) enable Carla's automatic cross-linking
- Omitting breaks the 20%+ internal CTR discoverability goal
- Must coordinate with Carla on series naming and consistent use across Part 1, 2, 3 articles

**Comparison Tables Solve Reader Confusion**
- Side-by-side tables (instructions vs. skills vs. agents) answer "when do I use X vs. Y?" upfront
- Reduces friction and reader frustration
- Should be standard in any article introducing related concepts

**Beginner Articles Need Visual Landmarks**
- Emoji callouts (✨, 🎯, 🔧, 📦, ✅, 🔍, 💡) improve scannability
- Step-by-step breakdown with numbered sections
- "Pro Tips" and "Troubleshooting" sections keep non-expert readers engaged
- Visual markers help readers skim and find key points quickly

**Microsoft 365 Template Pattern Established**
- Problem opener → Feature overview → Setup/licensing → Practical examples → Pro tips
- Licensing upfront prevents IT admin roadblocks
- Business professional tone and "executive assistant" framing resonate with audience
- Template reduces writing time for future M365 articles (Teams, Word, Excel, PowerPoint)

**Editorial Workflow Validated**
- Feature branches + draft PRs work well for pre-publish quality gate
- Frontmatter publish dates + featured flag enable scheduled releases
- Norm's quality review in parallel with writing prevents merge conflicts

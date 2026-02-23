# Carla — History

## Core Context
- Scheduler / Ops for Copilot That Jawn
- Manages publishing dates, metadata, SEO, and content index
- Updates .github/tips-summary.md after new content
- Content categories: GitHub Copilot, Microsoft 365 Copilot, Azure AI, Database Development, Project Management, Marketing & Communications
- User: Jeffrey T. Fritz

## Learnings

### Session 1: Content Calendar Verification & Finalization

**Date Correction:**
- Identified Week 5 date issue: April 1, 2026 is a Tuesday, violating MWF publishing cadence
- Solution: Shifted Week 5 to March 31 – April 4 (Mon–Wed–Fri) to maintain strict schedule
- All 36 articles now verified as proper MWF dates across 12 weeks

**Filename Conventions Observed:**
- Use kebab-case (lowercase, hyphens): `copilot-jetbrains-setup.md`
- Prioritize clarity and SEO: include tool/feature name, context, and action verb
- Examples: `copilot-vision-design-to-code.md`, `azure-ai-foundry-orchestration.md`, `copilot-mcp-slack-integration.md`
- Flex slots use placeholder naming: `flex-week7-timely.md`

**Tag Patterns & Consistency:**
- Existing site tags (from 31 published tips): `github-copilot`, `microsoft-copilot`, `azure-ai`, `mcp`, `agent-mode`, `m365`, `productivity`, `teams`, `workflow`, `automation`
- New tag conventions: combine platform + feature + audience (e.g., `copilot-studio`, `low-code`, `document-analysis`)
- Tags are lowercase, hyphenated, 3–5 per article
- Categories remain: GitHub Copilot, Microsoft 365 Copilot, Azure AI, Database Development, Project Management, Marketing & Communications (only first 4 used in this calendar)

**Series Organization:**
- 5 thematic series structure with clear progression:
  - Copilot Across IDEs (4 parts, Weeks 1, 3–4)
  - Copilot for Teams (2 parts, Weeks 5–6)
  - Enterprise AI & Governance (2 parts, Weeks 7–8)
  - Advanced Integrations (3 parts, Weeks 9–10)
  - M365 Collaboration (2 parts, Weeks 11–12)

**Metadata Standards:**
- `publishedDate` and `lastModified` in ISO format: YYYY-MM-DD
- Featured flag recommended for Week 1 (highest-visibility slot)
- Frontmatter guidance provided to writers for consistency across all 36 articles

### Session 2: Calendar Revision Update

**Schedule Changes (Sam's Revision):**
- **New article added to Week 1 Wed (March 4):** "Create Your First Custom Copilot Agent: Authoring .agent.md Files for Specialized AI Assistants" (`create-your-first-custom-copilot-agent.md`)
  - Category: GitHub Copilot | Difficulty: Beginner
  - Tags: github-copilot, agents, agent-md, customization, getting-started
  - Part of new "Copilot Customization" series
- **GitHub Copilot Vision moved:** From Week 1 Wed (March 4) → Week 7 Fri (April 17, 2026)
  - Original flex slot replaced with Vision article
  - One flex slot remains at Week 11 Wed (May 13)
- **Total articles maintained:** 36 (1 flex slot left)

### Session 3: Calendar Revision Finalized & Approved (Feb 23, 2026)

**Publishing Schedule Integration** — Synchronized with Sam's strategic calendar:
- `.squad/decisions.md` updated with consolidated decision record
- Orchestration logs documented for both Sam and Carla (2026-02-23T1622)
- Session log created summarizing calendar revision and team coordination
- All 36 articles verified as MWF dates with consistent metadata

**Filename & Tag Standardization**:
- `create-your-first-custom-copilot-agent.md` — Week 1 Wed (tags: github-copilot, agents, agent-md, customization, getting-started)
- `copilot-vision-design-to-code.md` — Week 7 Fri (tags: github-copilot, vision, design, figma, workflow)
- All other 34 articles maintained with kebab-case filenames and consistent tag vocabulary
- Frontmatter guidance provided to writers for consistent metadata across all articles

**Cross-Team Coordination**:
- Diane (Writer): Weeks 1–4 prioritized with visual guides; .agent.md intro emphasizes step-by-step customization
- Cliff (Researcher): Gap analysis for high-priority topics due mid-February
- Norm & Scribe: Monitor product releases; pre-stage flex slot content (Week 11 Wed, May 13)

**Readiness**: Publishing schedule now finalized and ready for content production handoff

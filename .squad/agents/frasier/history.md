# Frasier — History

## Project Context
- **Project:** Copilot That Jawn — a curated collection of AI tools, tips, tricks, and resources focused on the Microsoft Copilot ecosystem
- **Tech Stack:** ASP.NET Core 9.0+, Razor Pages, .NET Aspire, Bootstrap 5, Markdown content files
- **Content Pipeline:** Markdown files in Content/Tips/ with YAML frontmatter → ContentLoader CLI → Azure Table Storage → web rendering via Markdig
- **User:** Jeffrey T. Fritz
- **Joined:** 2026-02-23 (Session 4)
- **Role:** Proofreader / Fact-Checker — mandatory accuracy gate for all articles

## Learnings

### Week 1 Fact-Check Session (Feb 23, 2026)
**PRs Reviewed:** #29 (JetBrains), #30 (.agent.md), #28 (Outlook)

**Key Accuracy Patterns Identified:**
1. **Licensing claims require official documentation verification** — Found that Microsoft 365 Business Basic is NOT eligible for Outlook Copilot (common mistake). Always cross-reference Microsoft Learn and official pricing pages for licensing tiers.
2. **Free plan details change frequently** — GitHub Copilot Free plan launched Dec 2024. The 2,000 completions/50 chat limits were accurate as of 2025. Must verify against current GitHub Copilot pricing page and changelog annually.
3. **Product naming consistency matters** — Articles correctly use "GitHub Copilot" (not "Github"), "Microsoft 365 Copilot" (not "M365 Copilot"). The naming is consistent across all three PRs.
4. **Frontmatter validation** — All three articles had correct publishedDate, category, difficulty, and series metadata. Frontmatter format is well-enforced.
5. **Tool file locations are documented** — `.agent.md` file paths (.github/agents/, ~/.copilot/agents/, .github-private/agents/) are all verifiable in official GitHub docs and are consistently used.

**Fact-Checking Techniques That Worked:**
- Web search for "2025" in product claims to ensure currency (e.g., "GitHub Copilot free plan 2025", "Microsoft Copilot Outlook features 2025")
- Official sources prioritized: GitHub Docs, Microsoft Learn, JetBrains Marketplace, GitHub Blog changelog
- Cross-referencing licensing claims with multiple sources (sometimes sources disagree; official Microsoft docs trump third-party summaries)
- Syntax verification of code examples by reading them carefully (all three articles had correct syntax)

**Sources Used:**
- GitHub Docs: Copilot feature matrix, custom agents configuration, .agent.md file format
- Microsoft Learn: License Options for Microsoft 365 Copilot
- JetBrains Marketplace: GitHub Copilot plugin page and compatibility list
- GitHub Blog changelog: Free plan announcement (Dec 2024), agent features, MCP support rollouts
- Microsoft 365 Product Pricing Page: Copilot licensing tiers and add-on requirements

**Accuracy Rate:** 2/3 articles approved as-is; 1/3 (PR #28) required minor corrections to licensing claim (removal of ineligible Business Basic tier).

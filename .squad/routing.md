# Routing Rules

## Content Strategy & Planning
- Content calendar, editorial direction, topic prioritization → **Sam**
- "What should we write about?" / "Plan the next batch" → **Sam**

## Writing & Drafting
- New tips, tutorials, guides, articles → **Diane**
- Markdown content creation in Content/ directory → **Diane**
- Frontmatter formatting, content structure → **Diane**

## Research & Fact-Checking
- Tool research, feature verification → **Cliff**
- Source gathering, accuracy checks → **Cliff**
- "What does X tool do?" / "Is this correct?" → **Cliff**

## Scheduling & Operations
- Publishing dates, content metadata, SEO → **Carla**
- Frontmatter dates, series organization → **Carla**
- "When should this go live?" / "Update the schedule" → **Carla**

## Review & Quality
- Content review, tone check, completeness → **Norm**
- "Review this tip" / "Is this ready?" → **Norm**
- Quality approval before publishing → **Norm**

## Proofreading & Accuracy
- Grammar, spelling, punctuation checks → **Frasier**
- Technical accuracy, fact verification → **Frasier**
- Code example validation, link checking → **Frasier**
- "Is this accurate?" / "Fact-check this" → **Frasier**
- Accuracy approval before publishing → **Frasier**

## Multi-Agent Patterns
- "Write a new tip about X" → **Cliff** (research) + **Diane** (draft) → **Norm** (quality) + **Frasier** (accuracy) → **Carla** (schedule)
- "Plan and write a tutorial" → **Sam** (plan) → **Diane** (write) → **Norm** (quality) + **Frasier** (accuracy) → **Carla** (schedule)
- "Team, create content for X" → Fan out to all relevant agents
- "Review this article" → **Norm** (quality) + **Frasier** (accuracy) in parallel

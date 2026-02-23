# Norm — History

## Core Context
- Reviewer / QA for Copilot That Jawn
- Reviews content for accuracy, tone, completeness, and frontmatter compliance
- Quality gate before publishing — can approve or reject with feedback
- User: Jeffrey T. Fritz

## Learnings

### Week 1 Quality Gate (March 7, 2026)

**Review Scope Established**
- Reviewing 3 draft PRs: #28 (Outlook), #29 (JetBrains), #30 (Custom Agent)
- Accuracy vs. official product documentation
- Tone consistency with existing tips
- Frontmatter compliance (all 8 required fields + series metadata for series articles)
- Series metadata alignment with editorial calendar
- Step-by-step clarity and beginner accessibility
- Example completeness and end-to-end walkthrough validation

**Frontmatter Compliance Pattern**
- Required fields: title, description, category, tags, difficulty, author, publishedDate, lastModified
- Series articles also require: series, part, featured
- All 3 Week 1 articles include complete frontmatter
- Publish dates declared in metadata (features frontmatter date + featured: true flag)

**Cross-Diane Quality Observations**
- Diane delivered 3 articles (4,880 total words) across 6 days
- All articles follow consistent beginner-friendly structure (problem opener → feature overview → setup → practical examples → pro tips)
- Consistent emoji callout pattern (✨, 🎯, 🔧, etc.) improves scannability
- Series metadata present and correctly formatted (enables Carla's cross-linking strategy)
- Tone shifts appropriately by audience (GitHub Copilot vs. Microsoft 365)

**Editorial Workflow Validation**
- Feature branch + draft PR model works well for pre-publish review
- Publish date declaration in frontmatter enables scheduled releases
- Quality gate in parallel with writing prevents delays
- Norm can review during Diane's next spawn cycle without bottleneck

### Week 1 Article Reviews (Feb 23, 2026)
**Pattern Observed**: Two of three Week 1 articles hit quality bar immediately. One had tone/guideline issue.

**Quality Patterns**:
- **PR #29 & #30**: Excellent foundational content. Strong learning arcs, beginner-appropriate, clear metadata. Both ready to publish without revision.
- **PR #28**: Strong technical content, but misapplied "jawn" in marketing copy as pejorative. The project guideline is to use "jawn terminology appropriately in UI text," not as slang for "problem" or "hassle."

**Key Learnings**:
- Writers are broadly nailing frontmatter compliance and beginner accessibility
- Series metadata (series, part, featured) is being used consistently and correctly
- Language tone is generally excellent—the "jawn" issue was isolated misapplication of local flavor, not systematic tone drift
- Articles with step-by-step structure (PR #30) and clear use-case examples (all three) perform well on readability and actionability

**Recommendation for Future Reviews**: Flag any regional/colloquial terminology that might alienate non-local readers, especially if used as a pejorative or casual negative descriptor.

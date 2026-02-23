# Frasier's Week 1 Fact-Check Report
**Date:** Feb 23, 2026  
**Session:** Week 1 Draft Article Reviews  
**Status:** Complete — 2 Approved, 1 Corrections Needed

---

## Summary

Completed full fact-check and proofread of all 3 Week 1 draft articles (PRs #29, #30, #28). Found one factual error in PR #28 licensing claim; PRs #29 and #30 are accurate and ready to publish.

---

## Article-by-Article Verdicts

### ✅ PR #29: "GitHub Copilot in JetBrains IDEs" 
**Verdict:** APPROVED — No corrections needed.

**Key Findings:**
- JetBrains IDE support list (IntelliJ IDEA, PyCharm, WebStorm, PhpStorm, Rider, etc.) verified against JetBrains Marketplace
- Free plan limits (2,000 completions, 50 chat requests/month) confirmed per GitHub Copilot Free plan announcement (Dec 2024)
- All keyboard shortcuts verified as accurate
- Code examples syntactically correct
- Grammar and spelling: clean

**Ready for publication:** 2026-03-02

---

### ✅ PR #30: "Create Your First Custom Copilot Agent (.agent.md)"
**Verdict:** APPROVED — No corrections needed.

**Key Findings:**
- `.agent.md` file format and YAML frontmatter fields verified against GitHub's official specification
- All three file location paths confirmed:
  - `.github/agents/` for repository-scoped agents ✅
  - `~/.copilot/agents/` for personal/user-scoped agents ✅
  - `.github-private/agents/` for organization-wide agents ✅
- Agent comparison table (instructions vs. skills vs. agents) accurate
- Code examples all syntactically correct
- Grammar and spelling: clean
- Frontmatter dates and metadata aligned with content calendar

**Ready for publication:** 2026-03-04

---

### 🔄 PR #28: "Microsoft Copilot in Outlook"
**Verdict:** CORRECTIONS NEEDED — One factual error in licensing section.

**Issue Found:**

**Location:** Section "1. Check Your Subscription and License"

**Problem:**
> "You'll need an eligible **Microsoft 365 subscription** (Business Standard, Business Premium, **Business Basic**, or Apps for business/enterprise)"

**Correction:**
Per Microsoft's current 2025 licensing documentation, **Business Basic is NOT an eligible subscription tier** for Outlook Copilot.

Eligible plans:
- ✅ Business Standard
- ✅ Business Premium
- ✅ Apps for Business (with add-on)
- ✅ Enterprise plans

**Action Required:** Remove "Business Basic" from the list.

**Optional Improvement:** Clarify upfront that Copilot is a separate add-on license (currently mentioned later but not prominent in licensing section).

**Other Content:** All features (summarize, draft, prioritization, action items, meeting prep) verified as accurate. Grammar and spelling clean.

**Ready for publication:** 2026-03-06 (once corrections are made)

---

## Quality Summary

| Article | Accuracy | Grammar | Frontmatter | Verdict |
|---------|----------|---------|------------|---------|
| PR #29 (JetBrains) | ✅ 100% | ✅ Clean | ✅ Correct | APPROVED |
| PR #30 (.agent.md) | ✅ 100% | ✅ Clean | ✅ Correct | APPROVED |
| PR #28 (Outlook) | ⚠️ 1 error | ✅ Clean | ✅ Correct | Corrections Needed |

**Overall:** 2/3 articles approved. 1/3 requires single-line correction (removal of ineligible license tier).

---

## Fact-Checking Methodology

### Sources Relied Upon
1. **GitHub Official Docs**: Feature matrix, custom agents spec, .agent.md format
2. **Microsoft Learn**: License Options for Microsoft 365 Copilot (primary source for licensing)
3. **JetBrains Marketplace**: GitHub Copilot plugin compatibility list
4. **GitHub Blog**: Changelogs (Free plan announcement, agent features, MCP support)
5. **Product Pricing Pages**: GitHub Copilot plans, Microsoft 365 Copilot licensing tiers

### Key Verification Steps
- Searched for product claims with "2025" to ensure currency (e.g., "GitHub Copilot free plan 2025")
- Cross-referenced licensing against multiple sources; official Microsoft docs prioritized over summaries
- Verified IDE support lists against official marketplace
- Validated code examples for syntactic correctness
- Checked free plan limits against GitHub's official pricing page
- Confirmed file paths and formats against GitHub documentation

---

## Recommendations for Future Articles

1. **Licensing Claims:** Always verify against official Microsoft Learn and GitHub Docs licensing pages. Licensing tiers change annually and are a common source of errors.
2. **Free Plan Details:** Check GitHub Copilot Free plan limits annually (launched Dec 2024; may have changes in 2026+).
3. **Product Naming:** Maintain consistency with official naming:
   - ✅ "GitHub Copilot" (not "Github")
   - ✅ "Microsoft 365 Copilot" (not "M365 Copilot")
   - ✅ "Microsoft Copilot in Outlook" (exact product name)
4. **IDE Support:** Reference JetBrains Marketplace for current IDE support list (changes with plugin updates).
5. **File Paths:** Verify against official GitHub Docs for custom agent and instruction file locations.

---

## Next Steps

- PR #29 (JetBrains): Ready to merge and publish 2026-03-02
- PR #30 (.agent.md): Ready to merge and publish 2026-03-04
- PR #28 (Outlook): Awaiting author revision of licensing section; ready for republication once corrected

---

**Reviewed by:** Frasier (Proofreader / Fact-Checker)  
**Date:** Feb 23, 2026

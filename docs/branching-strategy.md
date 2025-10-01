---
layout: default
title: Branching Strategy
nav_order: 8
parent: Development
---

# Branching Strategy
{: .no_toc }

This document outlines the Git branching strategy used in the FundraiseUpApi project, including branch types, naming conventions, and versioning behavior.

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Overview

The FundraiseUpApi project uses a **GitHub Flow-inspired branching strategy** with semantic versioning powered by GitVersion. This approach balances simplicity with the need for structured releases and proper version management.

## Branch Types

### 🏠 **Master Branch** (`master` / `main`)
- **Purpose**: Production-ready code and stable releases
- **Versioning**: Patch increments for hotfixes
- **Protection**: Protected branch, requires pull requests
- **Deployment**: Automatically deployed to production

```yaml
master:
  regex: ^master$|^main$
  increment: Patch
  is-mainline: true
  source-branches: []
```

**Characteristics:**
- Always contains production-ready code
- All commits should be thoroughly tested
- Direct commits are disabled (except for emergency hotfixes)
- Serves as the base for hotfix branches

### 🚧 **Development Branch** (`dev` / `develop`)
- **Purpose**: Integration branch for ongoing development
- **Versioning**: Minor increments with alpha pre-release tags
- **Deployment**: Deployed to development/staging environments

```yaml
dev:
  regex: ^dev$|^develop$
  tag: 'alpha'
  increment: Minor
  source-branches: ['master']
```

**Characteristics:**
- Contains the latest development changes
- May be unstable at times
- Features are merged here before going to master
- Versions like `1.2.0-alpha.5`

### 🧪 **Beta Branch** (`beta` / `release/`)
- **Purpose**: Release candidates and pre-production testing
- **Versioning**: Patch increments with beta pre-release tags
- **Deployment**: Deployed to staging/pre-production environments

```yaml
beta:
  regex: ^beta$|^release[/-]
  tag: 'beta'
  increment: Patch
  source-branches: ['dev', 'master']
```

**Characteristics:**
- Created from dev when preparing a release
- Code freeze for new features
- Only bug fixes and final polishing
- Versions like `1.2.0-beta.3`

### ⚡ **Feature Branches** (`feature/` / `features/`)
- **Purpose**: Development of new features or enhancements
- **Versioning**: Inherits from parent branch with branch name tag
- **Lifecycle**: Short-lived, merged via pull requests

```yaml
feature:
  regex: ^features?[/-]
  tag: useBranchName
  increment: Inherit
  source-branches: ['dev', 'master']
```

**Naming Convention:**
- `feature/user-authentication`
- `feature/donation-tracking`
- `features/api-rate-limiting`

**Characteristics:**
- Created from dev (or master for urgent features)
- Versions like `1.2.0-user-authentication.4`
- Should be regularly rebased/merged with parent branch
- Deleted after successful merge

### 🔥 **Hotfix Branches** (`hotfix/`)
- **Purpose**: Critical bug fixes for production issues
- **Versioning**: Patch increments, marked as release branch
- **Urgency**: High priority, fast-tracked to production

```yaml
hotfix:
  regex: ^hotfix[/-]
  increment: Patch
  source-branches: ['master']
  is-release-branch: true
```

**Naming Convention:**
- `hotfix/security-vulnerability`
- `hotfix/critical-api-bug`
- `hotfix/donation-processing-fix`

**Characteristics:**
- Created directly from master
- Merged back to both master and dev
- Bypasses normal development flow
- Immediate deployment after merge

### 🔄 **Pull Request Branches** (`pull/` / `pr/`)
- **Purpose**: Temporary branches created by CI/CD for pull requests
- **Versioning**: Inherits from source with PR tag and number
- **Automation**: Automatically managed by GitHub/CI systems

```yaml
pull-request:
  regex: ^(pull|pr)[/-]
  tag: 'pr'
  increment: Inherit
  source-branches: ['dev', 'master', 'feature', 'beta']
  tag-number-pattern: '[/-](?<number>\d+)'
```

**Characteristics:**
- Versions like `1.2.0-pr.123.2`
- Used for CI/CD testing and validation
- Automatically cleaned up after PR closure

## Workflow Examples

### 🆕 **New Feature Development**

```bash
# 1. Create feature branch from dev
git checkout dev
git pull origin dev
git checkout -b feature/user-dashboard

# 2. Develop and commit changes
git add .
git commit -m "Add user dashboard components"
git commit -m "Implement dashboard API endpoints"

# 3. Push and create pull request
git push origin feature/user-dashboard
# Create PR: feature/user-dashboard → dev

# 4. After review and merge, delete branch
git branch -d feature/user-dashboard
```

### 🚀 **Release Preparation**

```bash
# 1. Create beta branch from dev
git checkout dev
git pull origin dev
git checkout -b beta

# 2. Stabilize and fix bugs
git commit -m "Fix authentication edge case"
git commit -m "Update API documentation"

# 3. Push beta branch
git push origin beta
# Deploy to staging environment

# 4. When ready, merge to master
# Create PR: beta → master
```

### 🔥 **Hotfix Process**

```bash
# 1. Create hotfix from master
git checkout master
git pull origin master
git checkout -b hotfix/critical-security-fix

# 2. Implement fix
git commit -m "Fix SQL injection vulnerability +semver: patch"

# 3. Push and create PRs
git push origin hotfix/critical-security-fix
# Create PR: hotfix/critical-security-fix → master
# Create PR: hotfix/critical-security-fix → dev

# 4. Emergency merge and deploy
```

## Version Control Best Practices

### 📝 **Commit Message Conventions**

Use conventional commit messages for automatic version detection:

```bash
# Force major version bump (breaking changes)
git commit -m "Remove deprecated API endpoints +semver: breaking"

# Force minor version bump (new features)
git commit -m "Add donation analytics endpoint +semver: feature"

# Force patch version bump (bug fixes)
git commit -m "Fix null reference in client +semver: fix"

# Skip version increment
git commit -m "Update documentation +semver: none"
```

### 🏷️ **Version Examples**

Based on the branching strategy, here are example versions:

| Branch Type | Example Version | Description |
|-------------|----------------|-------------|
| `master` | `1.2.3` | Stable release |
| `dev` | `1.3.0-alpha.5` | Development preview |
| `beta` | `1.3.0-beta.2` | Release candidate |
| `feature/auth` | `1.3.0-auth.8` | Feature development |
| `hotfix/bug` | `1.2.4` | Critical fix |
| `pull/123` | `1.3.0-pr.123.1` | PR validation |

### 🚫 **Documentation-Only Changes**

The following paths are ignored for version increments:
- `docs/**` - All documentation files
- `*.md` - Markdown files (README, CHANGELOG, etc.)
- `.github/ISSUE_TEMPLATE/**` - GitHub issue templates

This means documentation updates won't trigger new versions.

## Branch Protection Rules

### 🛡️ **Recommended GitHub Settings**

**Master Branch:**
- ✅ Require pull request reviews (2 reviewers)
- ✅ Require status checks to pass
- ✅ Restrict pushes to matching branches
- ✅ Include administrators

**Dev Branch:**
- ✅ Require pull request reviews (1 reviewer)
- ✅ Require status checks to pass
- ❌ Allow force pushes (for rebasing)

## Integration with CI/CD

### 🔄 **Automated Workflows**

```yaml
# Example GitHub Actions trigger
on:
  push:
    branches: [master, dev]
  pull_request:
    branches: [master, dev]
```

### 📦 **Deployment Strategy**

| Branch | Environment | Trigger | Version |
|--------|-------------|---------|---------|
| `master` | Production | Push/Merge | Stable |
| `dev` | Development | Push/Merge | Alpha |
| `beta` | Staging | Push/Merge | Beta |
| `feature/*` | Review Apps | PR Open | Preview |

## Troubleshooting

### ❓ **Common Issues**

**Q: GitVersion says "No base version could be determined"**
A: Ensure you have at least one commit on master branch with a valid version tag.

**Q: Feature branch showing wrong version**
A: Check that the branch name matches the regex pattern `^features?[/-]`.

**Q: Hotfix not incrementing patch version**
A: Verify the branch is created from master and named `hotfix/description`.

**Q: Documentation changes creating new versions**
A: Check that your files match the ignore patterns in GitVersion.yml.

For more troubleshooting, see our [Troubleshooting Guide](troubleshooting.md).

---

## Quick Reference

### 🎯 **Branch Naming Cheat Sheet**

```bash
feature/add-user-authentication    ✅ Correct
features/new-dashboard            ✅ Correct
hotfix/critical-bug-fix           ✅ Correct
release/v1.2.0                    ✅ Correct (beta)
beta                              ✅ Correct
dev                               ✅ Correct
develop                           ✅ Correct
master                            ✅ Correct
main                              ✅ Correct

feat/new-feature                  ❌ Wrong (use feature/)
fix/bug                          ❌ Wrong (use hotfix/)
```

### 📋 **Workflow Checklist**

**Before Creating a Branch:**
- [ ] Pull latest changes from target branch
- [ ] Choose appropriate branch type
- [ ] Follow naming conventions
- [ ] Verify GitVersion configuration

**Before Merging:**
- [ ] Code review completed
- [ ] All tests passing
- [ ] Version increment is appropriate
- [ ] Documentation updated if needed
- [ ] No merge conflicts

This branching strategy ensures clean version history, predictable releases, and maintainable code evolution while supporting both planned releases and emergency hotfixes.
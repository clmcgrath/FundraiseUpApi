# Release Workflow Setup Guide

## Quick Setup Checklist

### 1. Configure Repository Secrets
```bash
# In GitHub repository: Settings → Secrets and variables → Actions
```

**Required Secrets:**
- `NUGET_API_KEY` - Get from nuget.org → Account Settings → API Keys

### 2. Create Production Environment
```bash
# In GitHub repository: Settings → Environments
```

1. Create environment named `production`
2. Add required reviewers (recommended)
3. Set deployment branches to `master` and `stable`

### 3. Configure Branch Protection
```bash
# In GitHub repository: Settings → Branches
```

**For `master` branch:**
- ✅ Require a pull request before merging
- ✅ Require approvals (1+)
- ✅ Require status checks to pass
- ✅ Require branches to be up to date

### 4. Test the Workflow

1. Create a feature branch
2. Make a small change (e.g., update README)
3. Create PR to `master`
4. Merge the PR
5. Check GitHub Actions for release workflow execution

## Expected Workflow Behavior

### On PR Merge to `master`:
1. ✅ Validates release eligibility
2. ✅ Runs full build and test suite (imports build-test.yml)
3. ✅ Creates GitHub release with automated notes
4. ✅ Publishes to GitHub Packages
5. ✅ Publishes to NuGet.org (stable versions only)
6. ✅ Updates CHANGELOG.md
7. ✅ Comments on PR with release links

### Version Examples:
- `master` → `1.0.0`, `1.0.1`, `1.1.0` (GitHub + NuGet.org)
- `stable` → `1.0.0-rc.1`, `1.1.0-rc.2` (GitHub + NuGet.org)
- `dev` → `1.1.0-beta.1` (GitHub + NuGet.org)
- `feature/xyz` → `1.1.0-alpha.1` (GitHub only)

## Security Features

- 🔒 Pinned GitHub Actions to commit SHAs
- 🔒 Minimal required permissions
- 🔒 Environment protection rules
- 🔒 Automatic package signing
- 🔒 Comprehensive validation steps

## Manual Release Trigger

If needed, you can manually trigger a release:

1. Go to GitHub Actions
2. Select "🚀 Release" workflow
3. Click "Run workflow"
4. Choose options and run

This bypasses the normal PR merge trigger and can be useful for:
- Emergency releases
- Re-running failed releases
- Testing the release process

## Troubleshooting

**Release not triggered?**
- Ensure PR was merged (not just closed)
- Check target branch is `master` or `stable`
- Verify workflow file syntax

**NuGet publish failed?**
- Check NUGET_API_KEY secret is set
- Verify API key has correct permissions
- Ensure package name isn't already taken

**Need help?**
See [.github/PRODUCTION_ENVIRONMENT.md](.github/PRODUCTION_ENVIRONMENT.md) for detailed troubleshooting.
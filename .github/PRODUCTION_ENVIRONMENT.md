# Production Environment Configuration
# This file documents the production environment setup for GitHub Actions

## Required Secrets

The release workflow requires the following secrets to be configured in your GitHub repository:

### Repository Secrets
- `NUGET_API_KEY` - API key for publishing to NuGet.org
- `GITHUB_TOKEN` - Automatically provided by GitHub Actions (no setup required)

### Setting up NuGet.org API Key

1. Go to [NuGet.org](https://www.nuget.org/)
2. Sign in with your account
3. Go to Account Settings → API Keys
4. Create a new API key with:
   - **Key Name**: `FundraiseUpApi-GitHub-Actions`
   - **Select Scopes**: `Push new packages and package versions`
   - **Select Packages**: Choose existing packages or `*` for all packages
   - **Glob Pattern**: `FundraiseUp.*`

5. Copy the generated API key
6. In GitHub repository settings:
   - Go to Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Paste the API key
   - Click "Add secret"

## Environment Protection Rules

The workflow uses a `production` environment with the following recommended protection rules:

### GitHub Repository Settings
1. Go to Settings → Environments
2. Create environment named `production`
3. Configure protection rules:
   - **Required reviewers**: Add team members who can approve releases
   - **Wait timer**: 0 minutes (optional: add delay for review)
   - **Deployment branches**: Selected branches → `master`, `stable`

### Branch Protection Rules
1. Go to Settings → Branches
2. Add rule for `master`:
   - Require a pull request before merging
   - Require approvals: 1+
   - Require status checks to pass before merging
   - Require branches to be up to date before merging
   - Include administrators

## Workflow Triggers

The release workflow will run when:

1. **Pull Request Merged**: A PR to `master` or `stable` is merged
2. **Manual Trigger**: Using workflow_dispatch in GitHub Actions UI

### Automatic Releases
- Only merged PRs to protected branches (`master`, `stable`, `dev`) trigger releases
- Pre-release versions (containing `-`) are created as GitHub pre-releases
- **Stable versions**: Published to GitHub Packages + NuGet.org
- **Beta versions** (`-beta`): Published to GitHub Packages + NuGet.org (as pre-release)
- **Release candidates** (`-rc`): Published to GitHub Packages + NuGet.org (as pre-release)
- **Alpha and other pre-releases**: Published to GitHub Packages only

### Manual Releases
- Can be triggered from GitHub Actions → Release workflow → "Run workflow"
- Includes option to force release regardless of branch
- Includes option to override release type (major/minor/patch)

## Package Publishing Strategy

### GitHub Packages
- **All releases** (including pre-releases) are published
- Available immediately after workflow completion
- Requires authentication for consumption
- Good for internal testing and CI/CD

### NuGet.org
- **Stable releases** (no pre-release suffix) are published
- **Beta releases** (`-beta` suffix) are published as pre-releases
- **Release candidates** (`-rc` suffix) are published as pre-releases
- **Alpha and other pre-releases** are NOT published to NuGet.org
- Public consumption without authentication
- May take 15-30 minutes to appear in search/feeds
- Preferred for public distribution

## Version Management

Versions are automatically calculated using GitVersion based on:

- **Commit messages**: Following conventional commits
- **Branch name**: Different branches get different pre-release labels
- **GitVersion.yml**: Configuration for version calculation

### Version Examples
- `master` branch → `1.2.3` (GitHub Packages + NuGet.org)
- `stable` branch → `1.2.3-rc.1` (GitHub Packages + NuGet.org)
- `dev` branch → `1.3.0-beta.1` (GitHub Packages + NuGet.org)
- `feature/xyz` branch → `1.3.0-alpha.1` (GitHub Packages only)

## Security Considerations

- All GitHub Actions are pinned to specific commit SHAs
- Minimal required permissions are specified
- Secrets are only accessible during release job
- Environment protection prevents unauthorized releases
- Package signing is configured for NuGet packages

## Monitoring and Validation

The workflow includes comprehensive validation:

1. **Pre-flight checks**: Version validation, changelog extraction
2. **Build validation**: Full build and test suite
3. **Package validation**: Contents verification
4. **Post-release validation**: GitHub release and NuGet availability
5. **Release summary**: Detailed summary in workflow output

## Troubleshooting

### Common Issues

**Release not triggered**: Check that PR was merged (not closed) to protected branch

**NuGet publish failed**: Verify NUGET_API_KEY secret is set and has correct permissions

**Version already exists**: GitVersion calculated a version that already exists as a tag

**Package validation failed**: Build artifacts missing or incorrectly named

### Debugging Steps

1. Check workflow logs in GitHub Actions
2. Verify all required secrets are configured
3. Ensure branch protection rules allow the workflow
4. Check GitVersion configuration and commit messages
5. Validate CHANGELOG.md format for release notes extraction

### Manual Recovery

If a release fails partway through:

1. Check if GitHub release was created
2. Check if packages were published to GitHub Packages
3. Check if packages were published to NuGet.org
4. Manually complete missing steps if needed
5. Re-run workflow if safe to do so
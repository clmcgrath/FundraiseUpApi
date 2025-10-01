# FundraiseUpApi Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-09-29

## Active Technologies
- C# (.NET Standard 2.0 + .NET 6+) + System.Net.Http, Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration (001-i-want-create)

## Project Structure
```
src/
tests/
```

## Commands
# Add commands for C# (.NET Standard 2.0 + .NET 6+)

## Code Style
C# (.NET Standard 2.0 + .NET 6+): Follow standard conventions

## Recent Changes
- 001-i-want-create: Added C# (.NET Standard 2.0 + .NET 6+) + System.Net.Http, Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration

<!-- MANUAL ADDITIONS START -->

## Documentation Maintenance Guidelines

### Changelog Management
- **Always update CHANGELOG.md** when making any significant changes to the project
- **Follow Keep a Changelog format** with proper versioning and categorization
- **Update the [Unreleased] section** with new features, fixes, and improvements
- **Categorize changes** using: Added, Changed, Deprecated, Removed, Fixed, Security
- **Include impact description** for breaking changes or major features

### Documentation Synchronization
- **Maintain consistency** between README.md and docs/ GitHub Pages site
- **When updating README.md**, also update corresponding documentation in docs/
- **Key files to synchronize**:
  - README.md roadmap ↔ docs/roadmap.md
  - README.md examples ↔ docs/examples.md or docs/getting-started.md
  - README.md installation ↔ docs/getting-started.md
  - README.md configuration ↔ docs/configuration.md
- **Cross-reference documentation** to avoid duplication where possible
- **Update navigation** in docs/index.md when adding new documentation pages

### Release Notes Automation
- **CHANGELOG.md drives NuGet release notes** through CI/CD automation
- **Write clear, user-focused changelog entries** that will appear in package descriptions
- **Avoid internal implementation details** in changelog entries
- **Focus on user-facing changes** and their benefits

### Version Management
- **GitVersion.yml controls all versioning** - changes to branches affect version calculation
- **Document breaking changes clearly** in changelog with migration guidance
- **Update roadmap.md** when features are completed or priorities change
- **Coordinate version bumps** with changelog entries for clarity

<!-- MANUAL ADDITIONS END -->
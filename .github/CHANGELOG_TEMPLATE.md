# Changelog Template

Use this template for adding new changelog entries:

## [Version] - YYYY-MM-DD

### ✨ Added
- New features or functionality

### 🔄 Changed  
- Changes to existing functionality

### 🐛 Fixed
- Bug fixes

### 🗑️ Deprecated
- Features that will be removed in future versions

### ❌ Removed
- Features removed in this version

### 🔒 Security
- Security-related changes

---

## Changelog Guidelines

### Format Rules
- Use [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) format
- Follow [Semantic Versioning](https://semver.org/spec/v2.0.0.html)
- Group changes into categories (Added, Changed, Fixed, etc.)
- Use emoji icons for visual categorization
- Include dates in YYYY-MM-DD format

### Writing Style
- Use imperative mood ("Add feature" not "Added feature")
- Be specific and concise
- Include relevant issue or PR numbers when applicable
- Focus on user-facing changes
- Explain breaking changes clearly

### Categories
- **✨ Added**: New features
- **🔄 Changed**: Changes to existing functionality  
- **🐛 Fixed**: Bug fixes
- **🗑️ Deprecated**: Soon-to-be removed features
- **❌ Removed**: Removed features
- **🔒 Security**: Security fixes or improvements

### Examples

#### Good Entries
```markdown
### ✨ Added
- Rate limiting with Queue, Retry, and Exception strategies for handling FundraiseUp's 3 concurrent request limit
- Professional Jekyll documentation site with full-text search and responsive design
- Thread-safe rate limiting that works across all connection pooling strategies

### 🔄 Changed
- Migrated from Codecov to Codacy for unified code quality analysis and coverage reporting
- Updated all response models to match actual FundraiseUp API specification
- Enhanced GitHub Actions workflows with pinned commit SHAs for security

### 🐛 Fixed
- Fixed JSON serialization issues with decimal amounts in donation responses
- Resolved thread safety issues in concurrent request handling
- Corrected pagination cursor handling for large result sets
```

#### Avoid These
```markdown
### Added
- Stuff was added
- Various improvements
- Bug fixes and enhancements
```

### Unreleased Section
- Keep an "Unreleased" section at the top for ongoing work
- Move items from "Unreleased" to versioned sections when releasing
- Include notable unreleased changes for transparency

### Version Numbers
- Use semantic versioning (MAJOR.MINOR.PATCH)
- Link version numbers to GitHub releases when available
- Include planned release dates for upcoming versions
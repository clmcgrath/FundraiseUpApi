# FundraiseUp .NET Client Library Documentation

This directory contains the complete documentation for the FundraiseUp .NET Client Library, built with Jekyll using the Just the Docs theme.

## 📁 Structure

```
docs/
├── _config.yml                           # Jekyll configuration
├── index.md                              # Home page
├── getting-started.md                    # Installation and setup
├── configuration.md                      # Configuration options
├── RATE_LIMITING_CONNECTION_POOLING.md   # Advanced rate limiting guide
├── EXAMPLES.md                           # Usage examples
├── api-reference.md                      # API reference index
├── error-handling.md                     # Exception handling
└── performance.md                        # Performance optimization
```

## 🚀 Local Development

To run the documentation site locally:

```bash
# Install Jekyll (one time setup)
gem install bundler jekyll

# Create Gemfile
echo 'source "https://rubygems.org"' > Gemfile
echo 'gem "github-pages", group: :jekyll_plugins' >> Gemfile
echo 'gem "just-the-docs"' >> Gemfile

# Install dependencies
bundle install

# Serve locally
bundle exec jekyll serve

# View at http://localhost:4000
```

## 🎨 Theme Configuration

The documentation uses [Just the Docs](https://just-the-docs.github.io/just-the-docs/) theme with:

- **Search enabled** - Full-text search across all documentation
- **Navigation structure** - Automatic navigation from front matter
- **Syntax highlighting** - Code blocks with language support
- **Responsive design** - Mobile-friendly layout
- **GitHub integration** - Edit links and repository information

## 📝 Writing Guidelines

### Front Matter

All documentation files should include proper front matter:

```yaml
---
layout: default
title: Page Title
nav_order: 1
description: "Page description for SEO"
---
```

### Navigation Order

Pages are ordered using the `nav_order` field:

1. Home (index.md)
2. Getting Started 
3. Configuration
4. Rate Limiting & Connection Pooling
5. Examples
6. API Reference
7. Error Handling
8. Performance Guide

### Code Examples

Use proper syntax highlighting:

````markdown
```csharp
// C# code example
var client = new FundraiseUpClient("api-key");
```
````

### Callouts

Use Just the Docs callouts for important information:

```markdown
{: .warning }
> Important warning message

{: .note }
> Informational note

{: .highlight }
> Highlighted information
```

## 🔗 Links

- [Live Documentation](https://clmcgrath.github.io/FundraiseUpApi/)
- [GitHub Repository](https://github.com/clmcgrath/FundraiseUpApi)
- [Just the Docs Theme](https://just-the-docs.github.io/just-the-docs/)
- [Jekyll Documentation](https://jekyllrb.com/docs/)
# Contributing Guide

Thank you for your interest in contributing to the FundraiseUp .NET Client Library! This document provides guidelines and information for contributors.

## 🤝 How to Contribute

### Reporting Issues

1. **Search existing issues** first to avoid duplicates
2. **Use the issue templates** when creating new issues
3. **Provide detailed information**:
   - Your .NET version and target framework
   - Library version
   - Complete error message and stack trace
   - Minimal code example that reproduces the issue
   - Steps to reproduce

### Suggesting Enhancements

1. **Check the roadmap** in GitHub issues/projects
2. **Create an issue** with the "enhancement" label
3. **Describe the use case** and expected behavior
4. **Explain why it would be valuable** to the community

### Contributing Code

#### Prerequisites

- .NET 6.0 SDK or later
- Git
- IDE: Visual Studio 2022, VS Code, or JetBrains Rider

#### Getting Started

1. **Fork the repository**
2. **Clone your fork**:
   ```bash
   git clone https://github.com/your-username/FundraiseUpApi.git
   cd FundraiseUpApi
   ```

3. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

4. **Install dependencies**:
   ```bash
   dotnet restore
   ```

5. **Run tests to ensure everything works**:
   ```bash
   dotnet test
   ```

#### Development Workflow

1. **Make your changes**
2. **Add tests** for new functionality
3. **Ensure all tests pass**:
   ```bash
   dotnet test
   ```

4. **Check code formatting**:
   ```bash
   dotnet format
   ```

5. **Update documentation** if needed
6. **Commit your changes** with a clear message:
   ```bash
   git commit -m "feat: add support for new API endpoint"
   ```

7. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

8. **Create a Pull Request**

## 📝 Code Standards

### Coding Style

- Follow standard C# conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and single-purpose
- Use async/await properly with ConfigureAwait(false)

### Testing Requirements

- **Unit tests required** for all new functionality
- **Test coverage** should be maintained or improved
- **Use descriptive test names** that explain the scenario
- **Follow AAA pattern**: Arrange, Act, Assert

```csharp
[Test]
public async Task CreateDonation_WithValidRequest_ShouldReturnDonationResponse()
{
    // Arrange
    var request = new CreateDonationRequest { /* ... */ };
    var mockSetup = new HttpClientMockSetup();
    mockSetup.SetupSuccessResponse("/v1/donations", mockResponse);

    // Act
    var result = await client.Donations.Create(request).ExecuteAsync();

    // Assert
    Assert.That(result.Id, Is.Not.Null);
    Assert.That(result.Amount, Is.EqualTo("100.00"));
}
```

### Documentation Standards

- **Update README** if adding major features
- **Add XML documentation** for new public APIs
- **Include code examples** in documentation
- **Update changelog** for notable changes

## 🏗️ Project Structure

```
src/
├── FundraiseUp.Client/           # Main library
│   ├── Models/                   # Request/Response models
│   ├── Operations/               # API operation implementations
│   ├── Configuration/            # Client configuration
│   ├── Exceptions/               # Custom exception types
│   ├── Extensions/               # DI extension methods
│   ├── Interfaces/               # Interface definitions
│   └── Utilities/                # Helper classes

tests/
├── FundraiseUp.Client.Tests/    # Unit tests
│   ├── UnitTests/               # Unit test implementations
│   ├── Contracts/               # Contract tests
│   └── TestHelpers/             # Test utilities
└── FundraiseUp.Client.TestHelpers/ # Shared test utilities

docs/                            # Documentation
├── *.md                        # Guide files
└── _config.yml                 # Jekyll configuration
```

## 🧪 Testing Guidelines

### Unit Tests

- **Mock external dependencies** using Moq or similar
- **Test both success and failure scenarios**
- **Use the provided test helpers** for HTTP mocking
- **Keep tests isolated** and independent

### Integration Tests

- **Use test API keys** only
- **Mark with `[Category("Integration")]`** attribute
- **Expect these to be skipped** in CI/CD pipelines
- **Clean up test data** when possible

### Running Tests

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter "Category!=Integration"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🔄 Pull Request Process

### Before Submitting

- [ ] All tests pass
- [ ] Code follows project conventions
- [ ] Documentation updated
- [ ] Changelog updated (if applicable)
- [ ] No merge conflicts

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated (if applicable)
- [ ] All tests pass

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Changes work with both .NET Standard 2.0 and .NET 6+
```

### Review Process

1. **Automated checks** must pass (build, tests, linting)
2. **Maintainer review** for code quality and design
3. **Discussion** if changes needed
4. **Approval and merge** when ready

## 📋 Development Setup

### Recommended Tools

- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider
- **Extensions**: 
  - EditorConfig
  - GitLens (VS Code)
  - SonarLint

### Local Environment

```bash
# Clone repository
git clone https://github.com/clmcgrath/FundraiseUpApi.git
cd FundraiseUpApi

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Format code
dotnet format
```

### Configuration for Testing

Create `appsettings.Development.json` in test project:

```json
{
  "FundraiseUp": {
    "TestApiKey": "test_your-development-api-key"
  }
}
```

Or use user secrets:

```bash
dotnet user-secrets set "FundraiseUp:TestApiKey" "test_your-key" --project tests/FundraiseUp.Client.Tests
```

## 🚀 Release Process

### Versioning

We follow [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features, backward compatible
- **PATCH**: Bug fixes, backward compatible

### Release Checklist

- [ ] Version number updated
- [ ] Changelog updated
- [ ] All tests passing
- [ ] Documentation reviewed
- [ ] NuGet package metadata verified
- [ ] Release notes prepared

## 🆘 Getting Help

### For Contributors

- **GitHub Discussions**: General questions and ideas
- **GitHub Issues**: Bug reports and feature requests
- **Code Review**: Feedback on pull requests

### For Maintainers

- **Triage Issues**: Weekly issue review
- **Review PRs**: Timely pull request reviews
- **Release Management**: Coordinate releases
- **Documentation**: Keep docs updated

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the [MIT License](LICENSE).

## 🎉 Recognition

Contributors are recognized in:

- GitHub contributors list
- Release notes (for significant contributions)
- Special mentions in changelog

Thank you for helping make the FundraiseUp .NET Client Library better! 🙏
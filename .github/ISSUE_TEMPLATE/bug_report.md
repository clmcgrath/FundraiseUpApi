---
name: Bug Report
about: Create a report to help us improve
title: '[BUG] '
labels: bug
assignees: ''
---

## Bug Description
A clear and concise description of what the bug is.

## To Reproduce
Steps to reproduce the behavior:
1. Configure client with '...'
2. Call method '...'
3. With parameters '...'
4. See error

## Expected Behavior
A clear and concise description of what you expected to happen.

## Actual Behavior
What actually happened, including any error messages.

## Code Example
```csharp
// Minimal code example that reproduces the issue
var client = new FundraiseUpClient("test_key");
var result = await client.Donations.Create(request).ExecuteAsync();
```

## Environment
- **Library Version**: [e.g., 1.0.0]
- **Target Framework**: [e.g., .NET 6.0, .NET Standard 2.0]
- **Operating System**: [e.g., Windows 10, Ubuntu 20.04]
- **IDE**: [e.g., Visual Studio 2022, VS Code]

## Additional Context
Add any other context about the problem here, such as:
- Is this happening in development or production?
- Are you using test or live API keys?
- Any specific configuration settings?
- Stack trace or log output?

## Possible Solution
If you have ideas for fixing the issue, please describe them here.
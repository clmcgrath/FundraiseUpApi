---
layout: default
title: Roadmap
nav_order: 12
---

# Roadmap
{: .no_toc }

Our development roadmap outlines planned features and improvements for the FundraiseUpApi library.

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## 🎯 Development Philosophy

The FundraiseUpApi roadmap focuses on realistic, achievable goals that enhance developer experience while respecting the constraints and capabilities of the FundraiseUp REST API.

## 📈 Upcoming Features

### Advanced Filtering and Search Capabilities
**Status**: Planned  
**Focus**: Enhanced query capabilities within FundraiseUp API constraints

#### Planned Features
- **Enhanced Query Parameters**: More sophisticated filtering options for donations, campaigns, and supporters
- **Improved Search Functionality**: Better search capabilities across fundraising data
- **Advanced Date/Time Filtering**: Enhanced date range and time-based filtering options
- **Custom Field Support**: Better handling of custom fields and metadata in queries
- **Query Building Helpers**: LINQ-like query building extensions for cleaner code

#### Benefits
- More flexible data retrieval options
- Reduced network calls through better filtering
- Improved developer experience with intuitive query building

---

### Enhanced Caching and Performance Optimizations
**Status**: Planned  
**Focus**: Performance improvements and intelligent caching strategies

#### Planned Features
- **Response Caching**: Intelligent caching of read-only data (campaigns, supporters)
- **Connection Pool Optimizations**: Enhanced HttpClient connection management
- **Memory Usage Improvements**: Reduced memory footprint for large datasets
- **Async Pattern Enhancements**: Better async/await implementations throughout
- **Performance Monitoring**: Built-in performance metrics and monitoring hooks

#### Benefits
- Faster response times for frequently accessed data
- Reduced API call volume through intelligent caching
- Better resource utilization and lower memory usage
- Enhanced observability for performance troubleshooting

---

### Improved Error Handling and Retry Strategies  
**Status**: Planned  
**Focus**: Robust error handling and recovery mechanisms

#### Planned Features
- **Circuit Breaker Pattern**: Automatic failure detection and recovery
- **Advanced Retry Policies**: More sophisticated retry strategies beyond current rate limiting
- **Enhanced Exception Types**: More specific exception types with better context
- **Automatic Recovery**: Self-healing mechanisms for transient failures
- **Comprehensive Logging**: Enhanced logging for troubleshooting and monitoring

#### Benefits
- More resilient applications with better fault tolerance
- Improved debugging and troubleshooting capabilities  
- Reduced manual intervention for transient issues
- Better monitoring and alerting capabilities

---

### Modern .NET Features and Framework Updates
**Status**: Future Planning  
**Focus**: Leveraging latest .NET capabilities and framework modernization

#### Planned Features
- **.NET 10 LTS Support**: Target .NET 10 when released (November 2025)
- **Framework Consolidation**: Potentially drop .NET 6 support (EOL November 2024)
- **Source Generators**: Performance improvements through compile-time code generation
- **Nullable Reference Types**: Enhanced null safety throughout the library
- **Records and Modern C#**: Leverage latest C# language features
- **Minimal APIs Integration**: Better integration with ASP.NET Core minimal APIs
- **Native AOT Support**: Support for ahead-of-time compilation scenarios

#### Benefits
- Better performance through modern .NET optimizations
- Enhanced developer experience with latest language features
- Improved deployment scenarios with Native AOT
- Future-proofed architecture

---

## 🚫 Out of Scope

The following features are intentionally **not** included in our roadmap:

### Webhook Support
- **Reason**: Client libraries shouldn't handle incoming webhooks
- **Alternative**: Webhooks should be handled at the application level
- **Recommendation**: Use ASP.NET Core controllers or Azure Functions for webhook handling

### Bulk/Batch Operations  
- **Reason**: FundraiseUp API doesn't support batch operations
- **Alternative**: Individual API calls with proper rate limiting (already implemented)
- **Current Solution**: Our rate limiting handles concurrent requests efficiently

### GraphQL Support
- **Reason**: FundraiseUp uses REST API; GraphQL would add unnecessary complexity
- **Alternative**: REST API provides all necessary functionality
- **Current Solution**: Comprehensive REST API client with full endpoint coverage

---

## 🗓️ Timeline and Priorities

### Current Status (v1.0)
- ✅ **Complete**: Full REST API coverage for all FundraiseUp endpoints
- ✅ **Complete**: Thread-safe rate limiting with multiple strategies
- ✅ **Complete**: Comprehensive error handling and logging
- ✅ **Complete**: HttpClientFactory integration with connection pooling
- ✅ **Complete**: Professional documentation with GitHub Pages

### Current Focus (Infrastructure & Quality)
- 🔄 **In Progress**: CI/CD pipeline optimizations and workflow consolidation
- 🔄 **In Progress**: Code quality improvements with enhanced coverage reporting
- 🔄 **In Progress**: Testing infrastructure and reliability enhancements
- 📋 **Planned**: Documentation updates and maintenance automation

### Short Term (Next 3-6 months)
- 🎯 **Current Priority**: Stability improvements and CI/CD optimizations
- 📊 **Focus**: Code quality, testing coverage, and deployment pipeline enhancements

### Medium Term (6-12 months)  
- 🎯 **Next Features**: Advanced filtering and search capabilities
- 📊 **Focus**: Developer experience improvements and query flexibility

### Long Term (12-18 months)
- 🎯 **Performance Focus**: Caching and performance optimizations
- 🎯 **Resilience Focus**: Enhanced error handling and retry strategies
- 📊 **Focus**: Performance, resilience, and scalability

### Future Planning (18+ months)
- 🎯 **Modernization**: .NET framework updates and modern language features
- 📊 **Focus**: Framework modernization when .NET 10 LTS is available (November 2025)

---

## 🤝 Community Input

We welcome community feedback on our roadmap:

### How to Contribute to Planning
- **Feature Requests**: Submit ideas via [GitHub Discussions](https://github.com/clmcgrath/FundraiseUpApi/discussions)
- **Feedback**: Comment on existing roadmap items
- **Prioritization**: Help us understand what features matter most to your use cases
- **Real-World Scenarios**: Share your usage patterns to guide development

### Roadmap Updates
- **Quarterly Reviews**: Roadmap is reviewed and updated quarterly
- **Community Feedback**: Feature priorities may be adjusted based on community needs
- **API Changes**: Roadmap may be updated to reflect FundraiseUp API changes
- **Versioning**: We follow semantic versioning for all releases

---

## 📞 Questions About the Roadmap?

- **General Questions**: [GitHub Discussions](https://github.com/clmcgrath/FundraiseUpApi/discussions)
- **Feature Requests**: [GitHub Issues](https://github.com/clmcgrath/FundraiseUpApi/issues)
- **Timeline Questions**: Check our [project boards](https://github.com/clmcgrath/FundraiseUpApi/projects) for current progress

The roadmap reflects our commitment to providing a robust, performant, and developer-friendly FundraiseUp API client while maintaining realistic expectations and technical excellence.
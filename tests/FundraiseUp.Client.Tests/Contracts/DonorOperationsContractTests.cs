using System;
using System.Threading.Tasks;
using Xunit;

namespace FundraiseUp.Client.Tests.Contracts
{
    /// <summary>
    /// NOTE: Contract tests for supporter operations have been removed to avoid duplication.
    /// All supporter operation tests with proper mocking are located in:
    /// - UnitTests/DonorOperationsTests.cs (comprehensive mocked tests)
    /// 
    /// This prevents real API calls during testing while maintaining complete test coverage.
    /// </summary>
    public class DonorOperationsContractTests
    {
        [Fact(Skip = "Contract tests removed - see UnitTests/DonorOperationsTests.cs for comprehensive mocked tests")]
        public async Task ContractTests_RemovedToAvoidDuplication()
        {
            // All supporter operation testing is now handled by properly mocked unit tests
            // which provide the same coverage without making real API calls.
            await Task.CompletedTask;
        }
    }
}

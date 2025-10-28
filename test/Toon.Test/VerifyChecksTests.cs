using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Toon.Test
{
    public class VerifyChecksTests
    {
        [Fact]
        public Task Run() =>
            VerifyChecks.Run();
    }
}
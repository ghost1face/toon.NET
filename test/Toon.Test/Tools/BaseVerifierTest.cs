using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Toon.Test.Tools
{
    public abstract class BaseVerifierTest
    {
        protected virtual InnerVerifier BuildVerifier([CallerFilePath] string sourceFilePath = "")
        {
            var settings = new VerifySettings();

            settings.UseDirectory("./VerifiedOutputs");

            return Verifier.BuildVerifier(settings, sourceFilePath, useUniqueDirectory: false);
        }
    }
}
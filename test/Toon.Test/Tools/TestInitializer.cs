using System.Runtime.CompilerServices;
using VerifyTests;

namespace Toon.Test.Tools
{
    public static class TestInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            ClipboardAccept.Enable();
        }
    }
}
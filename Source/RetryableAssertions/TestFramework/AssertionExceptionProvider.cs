using System;

namespace Kontur.RetryableAssertions.TestFramework
{
    internal class AssertionExceptionProvider
    {
        private static readonly TestAssertionExceptions[] Infos =
        {
            new TestAssertionExceptions
            {
                FrameworkName = "nunit",
                AssemblyName = "nunit.framework",
                TypeName = "NUnit.Framework.AssertionException"
            },
            new TestAssertionExceptions {AssemblyName = "xx", TypeName = "yy"}
        };

        public static Type GetExceptionTypeFromSettings()
        {
            return null;
        }

        public static Type GetExceptionTypeFromRuntime()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var info in Infos)
                {
                    if (assembly.FullName.StartsWith(info.AssemblyName + ",", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var type = assembly.GetType(info.TypeName);
                        if (type != null)
                        {
                            return type;
                        }
                    }
                }
            }
            return null;
        }

        public static Type GetExceptionType()
        {
            return GetExceptionTypeFromSettings() ?? GetExceptionTypeFromRuntime();
        }

        private class TestAssertionExceptions
        {
            public string FrameworkName { get; set; }
            public string AssemblyName { get; set; }
            public string TypeName { get; set; }
        }
    }
}
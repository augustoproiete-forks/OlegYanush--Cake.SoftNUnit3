namespace Cake.SoftNUnit3
{
    using Cake.Common.Tools.NUnit;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    [CakeAliasCategory("Soft NUnit v3")]
    public static class SoftNUnit3Aliases
    {
        [CakeMethodAlias]
        public static string[] GetNUnit3NonPassedTests(this ICakeContext context, IEnumerable<FilePath> resultFilePaths)
        {
            List<string> total = new List<string>();

            foreach (var path in resultFilePaths)
            {
                var notPassed = XElement.Load(path.FullPath)
                    .XPathSelectElements("//test-case")
                    .Where(e => !e.Passed())
                    .Select(e => e.Attribute("fullname").Value);

                total.AddRange(notPassed);
            }

            context.Log.Verbose(Verbosity.Diagnostic, $"count of not passed tests = {total.Count}.");
            return total.Distinct().ToArray();
        }

        [CakeMethodAlias]
        public static void SoftNUnit3(this ICakeContext context, string pattern)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            FilePath[] array = context.Globber.GetFiles(pattern).ToArray();
            if (array.Length == 0)
            {
                context.Log.Verbose("The provided pattern did not match any files.");
                return;
            }
            context.SoftNUnit3(array, new NUnit3Settings());
        }

        /// <summary>
        /// Runs all NUnit unit tests in the assemblies matching the specified pattern,
        /// using the specified settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// NUnit3("./src/**/bin/Release/*.Tests.dll", new NUnit3Settings {
        ///     NoResults = true
        ///     });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void SoftNUnit3(this ICakeContext context, string pattern, NUnit3Settings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            FilePath[] array = context.Globber.GetFiles(pattern).ToArray<FilePath>();
            if (array.Length == 0)
            {
                context.Log.Verbose("The provided pattern did not match any files.");
                return;
            }
            context.SoftNUnit3(array, settings);
        }

        /// <summary>
        /// Runs all NUnit unit tests in the specified assemblies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <example>
        /// <code>
        /// NUnit3(new [] { "./src/Example.Tests/bin/Release/Example.Tests.dll" });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void SoftNUnit3(this ICakeContext context, IEnumerable<string> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            IEnumerable<FilePath> filePaths =
                from p in assemblies
                select new FilePath(p);
            context.SoftNUnit3(filePaths, new NUnit3Settings());
        }

        /// <summary>
        /// Runs all NUnit unit tests in the specified assemblies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <example>
        /// <code>
        /// var testAssemblies = GetFiles("./src/**/bin/Release/*.Tests.dll");
        /// NUnit3(testAssemblies);
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void SoftNUnit3(this ICakeContext context, IEnumerable<FilePath> assemblies)
        {
            context.SoftNUnit3(assemblies, new NUnit3Settings());
        }

        /// <summary>
        /// Runs all NUnit unit tests in the specified assemblies,
        /// using the specified settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// NUnit3(new [] { "./src/Example.Tests/bin/Release/Example.Tests.dll" }, new NUnit3Settings {
        ///     NoResults = true
        ///     });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void SoftNUnit3(this ICakeContext context, IEnumerable<string> assemblies, NUnit3Settings settings)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            IEnumerable<FilePath> filePaths =
                from p in assemblies
                select new FilePath(p);

            context.SoftNUnit3(filePaths, settings);
        }

        /// <summary>
        /// Runs all NUnit unit tests in the specified assemblies,
        /// using the specified settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// var testAssemblies = GetFiles("./src/**/bin/Release/*.Tests.dll");
        /// NUnit3(testAssemblies, new NUnit3Settings {
        ///     NoResults = true
        ///     });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void SoftNUnit3(this ICakeContext context, IEnumerable<FilePath> assemblies, NUnit3Settings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var runner = new SoftNUnit3Runner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            runner.Run(assemblies, settings);
        }
    }
}

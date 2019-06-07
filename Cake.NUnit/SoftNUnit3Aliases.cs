namespace Cake.NUnit
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

    [CakeAliasCategory("NUnit v3")]
    public static class SoftNUnit3Aliases
    {

        [CakeMethodAlias]
        public static FilePath CreateFile(this ICakeContext context, string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            System.IO.File.Create(path).Dispose();
            return new FilePath(path);
        }

        [CakeMethodAlias]
        public static string[] GetNUnit3FailedTests(this ICakeContext context, IEnumerable<FilePath> resultFilePaths)
        {
            var failed = new List<string>();

            foreach (var path in resultFilePaths)
            {
                var result = XElement.Load(path.FullPath);

                var cases = result.XPathSelectElements("//test-case")
                                  .ToList();

                failed.AddRange(cases.Where(e => e.Attribute("result").Value != "Passed")
                                     .Select(e => e.Attribute("fullname").Value));
            }
            return failed.Distinct()
                         .ToArray();
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

            var runner = new NUnit3SoftRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            runner.Run(assemblies, settings);
        }
    }
}

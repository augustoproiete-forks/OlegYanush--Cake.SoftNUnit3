namespace Cake.NUnit
{
    using Cake.Common.Tools.NUnit;
    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Tooling;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class NUnit3SoftRunner : Tool<NUnit3Settings>
    {
        private readonly ICakeEnvironment _environment;

        public NUnit3SoftRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
            _environment = environment;
        }

        private ProcessArgumentBuilder GetArguments(IEnumerable<FilePath> assemblyPaths, NUnit3Settings settings)
        {
            int? timeout;
            ProcessArgumentBuilder processArgumentBuilders = new ProcessArgumentBuilder();
            foreach (FilePath assemblyPath in assemblyPaths)
            {
                processArgumentBuilders.AppendQuoted(assemblyPath.MakeAbsolute(_environment).FullPath);
            }
            if (settings.Test != null)
            {
                processArgumentBuilders.Append(string.Concat("--test=", settings.Test));
            }
            if (settings.TestList != null)
            {
                processArgumentBuilders.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "--testlist={0}", settings.TestList.MakeAbsolute(_environment).FullPath));
            }
            if (settings.Where != null)
            {
                processArgumentBuilders.Append(string.Concat("--where \"", settings.Where, "\""));
            }
            if (settings.Timeout.HasValue)
            {
                timeout = settings.Timeout;
                processArgumentBuilders.Append(string.Concat("--timeout=", timeout.Value));
            }
            if (settings.Seed.HasValue)
            {
                timeout = settings.Seed;
                processArgumentBuilders.Append(string.Concat("--seed=", timeout.Value));
            }
            if (settings.Workers.HasValue)
            {
                timeout = settings.Workers;
                processArgumentBuilders.Append(string.Concat("--workers=", timeout.Value));
            }
            if (settings.StopOnError)
            {
                processArgumentBuilders.Append("--stoponerror");
            }
            if (settings.SkipNonTestAssemblies)
            {
                processArgumentBuilders.Append("--skipnontestassemblies");
            }
            if (settings.Work != null)
            {
                processArgumentBuilders.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "--work={0}", settings.Work.MakeAbsolute(this._environment).FullPath));
            }
            if (settings.OutputFile != null)
            {
                processArgumentBuilders.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "--out={0}", settings.OutputFile.MakeAbsolute(this._environment).FullPath));
            }
            if (this.HasResults(settings) && settings.NoResults)
            {
                throw new ArgumentException(string.Concat(this.GetToolName(), ": You can't specify both a results file and set NoResults to true."));
            }
            if (HasResults(settings))
            {
                foreach (NUnit3Result result in settings.Results)
                {
                    StringBuilder stringBuilder = new StringBuilder(result.FileName.MakeAbsolute(_environment).FullPath);
                    if (result.Format != null)
                    {
                        stringBuilder.AppendFormat(";format={0}", result.Format);
                    }
                    if (result.Transform != null)
                    {
                        stringBuilder.AppendFormat(";transform={0}", result.Transform.MakeAbsolute(_environment).FullPath);
                    }
                    processArgumentBuilders.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "--result={0}", stringBuilder));
                }
            }
            else if (settings.NoResults)
            {
                processArgumentBuilders.AppendQuoted("--noresult");
            }
            if (settings.Labels != NUnit3Labels.Off)
            {
                processArgumentBuilders.Append(string.Concat("--labels=", settings.Labels));
            }
            if (settings.TeamCity)
            {
                processArgumentBuilders.Append("--teamcity");
            }
            if (settings.NoHeader)
            {
                processArgumentBuilders.Append("--noheader");
            }
            if (settings.NoColor)
            {
                processArgumentBuilders.Append("--nocolor");
            }
            if (settings.Configuration != null)
            {
                processArgumentBuilders.AppendQuoted(string.Concat("--config=", settings.Configuration));
            }
            if (settings.Framework != null)
            {
                processArgumentBuilders.AppendQuoted(string.Concat("--framework=", settings.Framework));
            }
            if (settings.X86)
            {
                processArgumentBuilders.Append("--x86");
            }
            if (settings.DisposeRunners)
            {
                processArgumentBuilders.Append("--dispose-runners");
            }
            if (settings.ShadowCopy)
            {
                processArgumentBuilders.Append("--shadowcopy");
            }
            if (settings.Agents.HasValue)
            {
                timeout = settings.Agents;
                processArgumentBuilders.Append(string.Concat("--agents=", timeout.Value));
            }
            if (settings.Process != NUnit3ProcessOption.Multiple)
            {
                processArgumentBuilders.Append(string.Concat("--process=", settings.Process));
            }
            if (settings.AppDomainUsage != NUnit3AppDomainUsage.Default)
            {
                processArgumentBuilders.Append(string.Concat("--domain=", settings.AppDomainUsage));
            }
            if (settings.TraceLevel.HasValue)
            {
                NUnitInternalTraceLevel? traceLevel = settings.TraceLevel;
                processArgumentBuilders.Append(string.Concat("--trace=", traceLevel.Value.GetArgumentValue()));
            }
            if (settings.ConfigFile != null)
            {
                processArgumentBuilders.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "--configfile={0}", settings.ConfigFile.MakeAbsolute(this._environment).FullPath));
            }
            if (settings.Params != null && settings.Params.Count > 0)
            {
                foreach (KeyValuePair<string, string> param in settings.Params)
                {
                    processArgumentBuilders.AppendQuoted(string.Format(CultureInfo.InvariantCulture, "--params={0}={1}", param.Key, param.Value));
                }
            }
            return processArgumentBuilders;
        }

        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new string[] { "nunit3-console.exe" };
        }

        protected override string GetToolName()
        {
            return "NUnit3";
        }

        private bool HasResults(NUnit3Settings settings)
        {
            if (settings.Results == null)
            {
                return false;
            }
            return settings.Results.Count > 0;
        }

        protected override void ProcessExitCode(int exitCode)
        {
            string str;
            if (exitCode > 0)
            {
                exitCode = 0;
            }

            if (exitCode == -100)
            {
                str = "Unexpected error";
            }
            else
            {
                switch (exitCode)
                {
                    case -5:
                        {
                            str = "Unload error";
                            break;
                        }
                    case -4:
                        {
                            str = "Invalid test fixture";
                            break;
                        }
                    case -3:
                        {
                            str = "Unrecognised error";
                            break;
                        }
                    case -2:
                        {
                            str = "Invalid assembly";
                            break;
                        }
                    case -1:
                        {
                            str = "Invalid argument";
                            break;
                        }
                    case 0:
                        {
                            return;
                        }
                    default:
                        {
                            goto case -3;
                        }
                }
            }
            throw new CakeException(string.Format(CultureInfo.InvariantCulture, "{0}: {1} (exit code {2}).", this.GetToolName(), str, exitCode));
        }

        public void Run(IEnumerable<FilePath> assemblyPaths, NUnit3Settings settings)
        {
            if (assemblyPaths == null)
            {
                throw new ArgumentNullException("assemblyPaths");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            base.Run(settings, this.GetArguments(assemblyPaths, settings));
        }
    }
}

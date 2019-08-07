namespace Cake.SoftNUnit3
{
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;

    [CakeAliasCategory("File")]
    public static class FileAliases
    {
        [CakeMethodAlias]
        public static FilePath CreateFile(this ICakeContext context, string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            using (var file = System.IO.File.Create(path)) { }
            context.Log.Debug(Verbosity.Diagnostic, $"file with path {path} successfully created.");

            return new FilePath(path);
        }
    }
}

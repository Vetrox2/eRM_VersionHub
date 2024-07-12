using eRM_VersionHub.Models;
using eRM_VersionHub.Services;

namespace eRM_VersionHub_Tester.Services
{
    internal static class FileStructureGenerator
    {
        private static readonly string common = "./Disc1/", internalDisc = "eRMwewn", externalDisc = "eRMzewn", packages = "packages",
            appsFolder = "apps", appJson = "application.json";

        public static (string appsPath, string appJson, string internalPath, string externalPath) GenerateFileStructure()
        {
            PrepareDirectories();
            CreateAppJsons();
            File.WriteAllText(Path.Combine(common, "packages.txt"),
                "+module1\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   " +
                "+0.2\r\n-   -       test.zip\r\n-   -   \r\n-   +0.3\r\n-   -       test.zip\r\n-   -    " +
                "\r\n+module2\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n-   -     " +
                "\r\n+module3\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n-   -     " +
                "\r\n+module4\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       " +
                "\r\n+module5\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n-   -    ");

            string internalPackagesPath = Path.Combine(common, internalDisc, packages), externalPackagesPath = Path.Combine(common, externalDisc, packages);
            PackagesGenerator.Generate(internalPackagesPath, Path.Combine(common, "packages.txt"));
            return (Path.Combine(common, appsFolder), appJson, internalPackagesPath, externalPackagesPath);
        }

        public static void DeleteFileStructure()
        {
            if (Directory.Exists(common))
                Directory.Delete(common, true);
        }

        private static void PrepareDirectories()
        {
            Directory.CreateDirectory(common);
            Directory.CreateDirectory(Path.Combine(common, internalDisc));
            Directory.CreateDirectory(Path.Combine(common, internalDisc, packages));
            Directory.CreateDirectory(Path.Combine(common, externalDisc));
            Directory.CreateDirectory(Path.Combine(common, externalDisc, packages));
            Directory.CreateDirectory(Path.Combine(common, appsFolder));
            Directory.CreateDirectory(Path.Combine(common, appsFolder, "app1"));
            Directory.CreateDirectory(Path.Combine(common, appsFolder, "app2"));
            Directory.CreateDirectory(Path.Combine(common, appsFolder, "app3"));
        }
        private static void CreateAppJsons()
        {
            var app1 = new ApplicationJsonModel()
            {
                UniqueIdentifier = "app1",
                Name = "App1",
                Modules =
                [
                new ModuleJsonModel() { ModuleId = "module1", Optional = false },
                new ModuleJsonModel() { ModuleId = "module2", Optional = false }
                ]
            };
            var app2 = new ApplicationJsonModel()
            {
                UniqueIdentifier = "app2",
                Name = "App2",
                Modules =
                [
                new ModuleJsonModel() { ModuleId = "module3", Optional = false },
                new ModuleJsonModel() { ModuleId = "module2", Optional = true }
                ]
            };
            var app3 = new ApplicationJsonModel()
            {
                UniqueIdentifier = "app3",
                Name = "App3",
                Modules =
                [
                new ModuleJsonModel() { ModuleId = "module4", Optional = true },
                new ModuleJsonModel() { ModuleId = "module5", Optional = true }
                ]
            };

            File.WriteAllText(Path.Combine(common, appsFolder, "app1", appJson), app1.Serialize());
            File.WriteAllText(Path.Combine(common, appsFolder, "app2", appJson), app2.Serialize());
            File.WriteAllText(Path.Combine(common, appsFolder, "app3", appJson), app3.Serialize());
        }
    }
}

using eRM_VersionHub.Models;
using eRM_VersionHub.Services;

namespace eRM_VersionHub_Tester.Helpers
{
    public class FileStructureGenerator : IDisposable
    {
        private readonly string _randomGen;
        private readonly string _common;
        private readonly string _internalDisc;
        private readonly string _externalDisc;
        private readonly string _packages;
        private readonly string _appsFolder;
        private readonly string _appJson;

        public FileStructureGenerator()
        {
            _randomGen = Guid.NewGuid().ToString();
            _common = Path.Combine(Path.GetTempPath(), "TestDisc1", _randomGen);
            _internalDisc = Path.Combine(_common, "eRMwewn");
            _externalDisc = Path.Combine(_common, "eRMzewn");
            _packages = "packages";
            _appsFolder = "apps";
            _appJson = "application.json";
        }

        public (
            string appsPath,
            string appJson,
            string internalPath,
            string externalPath
        ) GenerateFileStructure()
        {
            try
            {
                PrepareDirectories();
                CreateAppJsons();

                string packagesFilePath = Path.Combine(_common, "packages.txt");
                string internalPackagesPath = Path.Combine(_internalDisc, _packages);
                string externalPackagesPath = Path.Combine(_externalDisc, _packages);

                File.WriteAllText(packagesFilePath, GeneratePackagesContent(true));
                PackagesGenerator.Generate(internalPackagesPath, packagesFilePath);

                File.WriteAllText(packagesFilePath, GeneratePackagesContent(false));
                PackagesGenerator.Generate(externalPackagesPath, packagesFilePath);

                return (
                    Path.Combine(_common, _appsFolder),
                    _appJson,
                    internalPackagesPath,
                    externalPackagesPath
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateFileStructure: {ex.Message}");
                throw;
            }
        }

        private void PrepareDirectories()
        {
            Directory.CreateDirectory(_common);
            Directory.CreateDirectory(_internalDisc);
            Directory.CreateDirectory(Path.Combine(_internalDisc, _packages));
            Directory.CreateDirectory(_externalDisc);
            Directory.CreateDirectory(Path.Combine(_externalDisc, _packages));
            Directory.CreateDirectory(Path.Combine(_common, _appsFolder));
            Directory.CreateDirectory(Path.Combine(_common, _appsFolder, "app1"));
            Directory.CreateDirectory(Path.Combine(_common, _appsFolder, "app2"));
            Directory.CreateDirectory(Path.Combine(_common, _appsFolder, "app3"));
        }

        private void CreateAppJsons()
        {
            var app1 = new ApplicationJsonModel()
            {
                UniqueIdentifier = "app1",
                Name = "App1",
                Modules = new List<ModuleJsonModel>
                {
                    new ModuleJsonModel() { ModuleId = "module1", Optional = false },
                    new ModuleJsonModel() { ModuleId = "module2", Optional = false }
                }
            };
            var app2 = new ApplicationJsonModel()
            {
                UniqueIdentifier = "app2",
                Name = "App2",
                Modules = new List<ModuleJsonModel>
                {
                    new ModuleJsonModel() { ModuleId = "module3", Optional = false },
                    new ModuleJsonModel() { ModuleId = "module2", Optional = true }
                }
            };
            var app3 = new ApplicationJsonModel()
            {
                UniqueIdentifier = "app3",
                Name = "App3",
                Modules = new List<ModuleJsonModel>
                {
                    new ModuleJsonModel() { ModuleId = "module4", Optional = true },
                    new ModuleJsonModel() { ModuleId = "module5", Optional = true }
                }
            };

            File.WriteAllText(
                Path.Combine(_common, _appsFolder, "app1", _appJson),
                app1.Serialize()
            );
            File.WriteAllText(
                Path.Combine(_common, _appsFolder, "app2", _appJson),
                app2.Serialize()
            );
            File.WriteAllText(
                Path.Combine(_common, _appsFolder, "app3", _appJson),
                app3.Serialize()
            );
        }

        private string GeneratePackagesContent(bool isInternal)
        {
            if (isInternal)
            {
                return "+module1\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   "
                    + "+0.2\r\n-   -       test.zip\r\n-   -   \r\n-   +0.3\r\n-   -       test.zip\r\n-   -   \r\n-   " +
                    "+0.4-prefix\r\n-   -       test.zip\r\n-   -    "
                    + "\r\n+module2\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n-   -   " +
                    "\r\n-   +0.4-prefix\r\n-   -       test.zip\r\n-   -     "
                    + "\r\n+module3\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n-   -     "
                    + "\r\n+module4\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       "
                    + "\r\n+module5\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n-   -    ";
            }
            else
            {
                return "\r\n+module2\r\n-   +0.1\r\n-   -       test.zip\r\n-   -       \r\n-   +0.2\r\n-   -       test.zip\r\n" +
                    "-   -   \r\n-   +0.4-prefix\r\n-   -       test.zip\r\n-   -     "
                    + "\r\n+module3\r\n-   +0.2\r\n-   -       test.zip\r\n-   -     ";
            }
        }

        public void Dispose()
        {
            DeleteFileStructure();
        }

        private void DeleteFileStructure()
        {
            try
            {
                if (Directory.Exists(_common))
                {
                    Directory.Delete(_common, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file structure: {ex.Message}");
                // Consider logging this error or handling it in a way that doesn't disrupt test execution
            }
        }
    }
}

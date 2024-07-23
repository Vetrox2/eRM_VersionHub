namespace eRM_VersionHub.Services
{
    public static class PackagesGenerator
    {
        public static void Generate(string basePath = @"Disc\eRMwewn\packages", string packagesInfoPath = @"Disc\packages.txt")
        {
            string currentFolder = "";
            string subfolderPath = "";
            Random r = new();

            if (!Directory.Exists(basePath))
            {
                Console.WriteLine("An error occurred: Base path doesn't exist!");
                Environment.Exit(1);
            }

            var info = new DirectoryInfo(basePath);

            if (info.GetDirectories().Length != 0)
            {
                return;
            }
            else if (!File.Exists(packagesInfoPath))
            {
                Console.WriteLine("An error occurred: packages.txt doesn't exist!");
                Environment.Exit(1);
            }

            using (StreamReader rw = new(packagesInfoPath))
            {
                while (!rw.EndOfStream)
                {
                    string line = rw.ReadLine();

                    if (line.StartsWith("+"))
                    {
                        currentFolder = line.TrimStart('+').Trim();
                        string mainFolderPath = Path.Combine(basePath, currentFolder);
                        Directory.CreateDirectory(mainFolderPath);
                    }
                    else if (line.StartsWith("-"))
                    {
                        string subfolderName = line.TrimStart('-').Trim();

                        if (subfolderName.StartsWith("+"))
                        {
                            subfolderName = subfolderName.TrimStart('+').Trim();
                        }

                        if (subfolderName.StartsWith("-"))
                        {
                            subfolderName = subfolderName.TrimStart('-').Trim();
                        }

                        if (string.IsNullOrEmpty(subfolderName))
                        {
                            continue;
                        }

                        if (char.IsDigit(subfolderName[0]))
                        {
                            subfolderPath = Path.Combine(basePath, currentFolder, subfolderName);
                            Directory.CreateDirectory(subfolderPath);
                        }

                        if (char.IsLetter(subfolderName[0]))
                        {
                            var filePath = Path.Combine(subfolderPath, subfolderName);
                            int size = r.Next(256, 10000);

                            byte[] buffer = new byte[size];
                            r.NextBytes(buffer);
                            File.WriteAllBytes(filePath, buffer);
                        }
                    }
                }
            }
        }
    }
}

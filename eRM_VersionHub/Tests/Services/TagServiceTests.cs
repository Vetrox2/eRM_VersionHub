using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub_Tester.Helpers;

namespace eRM_VersionHub_Tester.Services
{
    public class TagServiceTests : IAsyncLifetime
    {
        private readonly FileStructureGenerator _fileStructureGenerator = new FileStructureGenerator();
        private string appsPath, appJson, internalPath, externalPath;

        public Task InitializeAsync()
        {
            (appsPath, appJson, internalPath, externalPath) = _fileStructureGenerator.GenerateFileStructure();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _fileStructureGenerator.Dispose();
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("0.1", "0.1", "")]
        [InlineData("0.1-tag", "0.1", "tag")]
        [InlineData("0.2OF-tag", "0.2OF", "tag")]
        [InlineData("0.4OF-scope", "0.4OF", "scope")]
        [InlineData("1.454.2143.5452-scope", "1.454.2143.5452", "scope")]
        public void SplitVersion_ShouldSplitVersionCorrectly(string versionID, string expectedName, string expectedTag)
        {
            var (name, tag) = TagService.SplitVersionID(versionID);

            Assert.Equal(name, expectedName);
            Assert.Equal(tag, expectedTag);
        }

        [Theory]
        [InlineData("0.1", "0.1")]
        [InlineData("0.1-tag", "0.1")]
        [InlineData("0.2OF-tag", "0.2OF")]
        [InlineData("0.4OF-scope", "0.4OF")]
        [InlineData("1.454.2143.5452-scope", "1.454.2143.5452")]
        public void GetVersion_ShouldReturnVersionCorrectly(string versionID, string expected)
        {
            var name = TagService.GetVersionWithoutTag(versionID);

            Assert.Equal(name, expected);
        }

        [Theory]
        [InlineData("0.1", "")]
        [InlineData("0.1-tag", "tag")]
        [InlineData("0.2OF-tag", "tag")]
        [InlineData("0.4OF-scope", "scope")]
        [InlineData("1.454.2143.5452-blblbl", "blblbl")]
        public void GetTag_ShouldReturnTagCorrectly(string versionID, string expected)
        {
            var tag = TagService.GetTag(versionID);

            Assert.Equal(tag, expected);
        }

        [Theory]
        [InlineData("0.1", "", "0.1")]
        [InlineData("0.1-tag", "", "0.1")]
        [InlineData("0.2OF-tag", "", "0.2OF")]
        [InlineData("0.4OF-scope", "test", "0.4OF-test")]
        [InlineData("1.454.2143.5452-scope", "preview", "1.454.2143.5452-preview")]
        [InlineData("1.454.2143.5452", "preview", "1.454.2143.5452-preview")]
        [InlineData("1.454.2143.5452", "tag", "1.454.2143.5452-tag")]
        public void SwapTag_ShouldSwapTagCorrectly(string versionID, string newTag, string expected)
        {
            var newVersionID = TagService.SwapVersionTag(versionID, newTag);

            Assert.Equal(newVersionID, expected);
        }

        [Theory]
        [InlineData("0.1", "0.1", true)]
        [InlineData("0.1-tag", "0.1", true)]
        [InlineData("0.2OF-tag", "0.2OF", true)]
        [InlineData("0.4OF-scope", "0.4OF-preview", true)]
        [InlineData("0.4OF", "0.4OF-preview", true)]
        [InlineData("1.454.2143.5452", "1.454.2143.5452-test", true)]
        [InlineData("0.2", "0.1", false)]
        [InlineData("0.2-test", "0.1-test", false)]
        public void CompareVersions_ShouldCompareVersionsCorrectly(string versionID1, string versionID2, bool expected)
        {
            var result = TagService.CompareVersions(versionID1, versionID2);

            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData("module2", "0.1", "0.1", true)]
        [InlineData("module2", "0.1", "0.1-tag", true)]
        [InlineData("module2", "0.2", "0.2-tag", true)]
        [InlineData("module2", "0.4-prefix", "0.4-tag", true)]
        [InlineData("module2", "0.4-prefix", "0.4", true)]
        [InlineData("module2", "0.4-prefix", "0.2", false)]
        [InlineData("module2", "0.2", "0.1-tag", false)]
        [InlineData("module3", "0.2", "0.2-preview", true)]
        [InlineData("module3", "0.5", "0.5-preview", false)]
        [InlineData("module5", "0.1", "0.1-preview", false)]
        public void ChangeTagOnPath_ShouldChangeTagCorrectly(string moduleID, string versionID, string newVersionID, bool expected)
        {
            var result = TagService.ChangeTagOnPath(externalPath, moduleID, versionID, newVersionID);

            Assert.Equal(result, expected);
        }
    }
}

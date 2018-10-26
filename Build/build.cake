#tool "nuget:?package=GitVersion.CommandLine"
#addin nuget:?package=Cake.Coverlet
#tool nuget:?package=Codecov
#addin nuget:?package=Cake.Codecov
#tool nuget:?package=MSBuild.SonarQube.Runner.Tool
#addin nuget:?package=Cake.Sonar

var target = Argument("target", "Default");

string version, branch;
var buildPath = Directory("./build-artifacts");
var publishPath = buildPath + Directory("publish");
var releasePath = buildPath + Directory("release");
var testPath = buildPath + Directory("test");

var codecovToken = EnvironmentVariable("CODECOV_TOKEN");
var sonarCloudToken = EnvironmentVariable("SONARCLOUD_TOKEN");

Task("__Clean")
  .Does(() => {
      CleanDirectories(new DirectoryPath[]{
        buildPath
      });
      CleanDirectories("../source/**/bin");
      CleanDirectories("../source/**/obj");
      CleanDirectories("../tests/**/bin");
      CleanDirectories("../tests/**/obj");
  });

Task("__Versioning")
  .Does(() => {
    var gitVersion = GitVersion();
    version = gitVersion.NuGetVersion;
    branch = gitVersion.BranchName;
    Information(version);
    var files = GetFiles("../source/**/*.csproj");
    foreach (var file in files) {
      
      XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", gitVersion.AssemblySemVer);
      XmlPoke(file, "/Project/PropertyGroup/FileVersion", version);
      XmlPoke(file, "/Project/PropertyGroup/Version", version);
    }
    if (AppVeyor.IsRunningOnAppVeyor) {
      GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true, 
        OutputType = GitVersionOutput.BuildServer
      });
    }
  });

Task("__NugetRestore")
  .Does(() => {
    DotNetCoreRestore("../TrekkingForCharity.AlgoliaLocal.sln");
  });

Task("__Test")
  .Does(() => {
    var testFilePath = MakeAbsolute(File(testPath.ToString() + "/xunit-report.xml"));
    
    var testSettings = new DotNetCoreTestSettings {
      Configuration = "Release",
      Logger = string.Format("trx;LogFileName={0}", testFilePath)
    };

    var coveletSettings = new CoverletSettings {
      CollectCoverage = true,
      CoverletOutputFormat = CoverletOutputFormat.opencover,
      CoverletOutputDirectory = testPath,
      CoverletOutputName = "opencover.xml"
    };

    DotNetCoreTest("../tests/TrekkingForCharity.AlgoliaLocal.Tests/TrekkingForCharity.AlgoliaLocal.Tests.csproj", testSettings, coveletSettings);

    if (AppVeyor.IsRunningOnAppVeyor) {
      if (FileExists(testFilePath)) {
        AppVeyor.UploadTestResults(testFilePath, AppVeyorTestResultsType.XUnit);
      }
    }
  });

Task("__Publish")
  .Does(() => {
    var msbuildSettings = new MSBuildSettings {
      Verbosity = Verbosity.Minimal,
      ToolVersion = MSBuildToolVersion.VS2017,
      Configuration = "Release",
      PlatformTarget = PlatformTarget.MSIL
    };
    
    msbuildSettings.WithProperty("OutDir", MakeAbsolute(publishPath).ToString());
    MSBuild("../source/TrekkingForCharity.AlgoliaLocal/TrekkingForCharity.AlgoliaLocal.csproj", msbuildSettings);
  });

Task("__Package")
  .Does(() => {
    var nugetPackSettings = new NuGetPackSettings {
      Version = version,
      BasePath = publishPath,
      OutputDirectory = releasePath
		};

		NuGetPack("./TrekkingForCharity.AlgoliaLocal.nuspec", nugetPackSettings);
  });

Task("__ProcessDataForThirdParties")
  .Does(() => {
    if (AppVeyor.IsRunningOnAppVeyor) {
      var settings = new SonarBeginSettings{
        Url = "https://sonarcloud.io",
        Key = "t4c-al",
        Login = sonarCloudToken,        
        Verbose = true,
        Organization = "trekking-for-charity"
      };
      if (FileExists("./build-artifacts/test/opencover.xml")) {
        settings.OpenCoverReportsPath = MakeAbsolute(File("./build-artifacts/test/opencover.xml")).ToString();
      }

      if (AppVeyor.IsRunningOnAppVeyor) {
        int? pullRequestKey = null;
        int result;
        if (!string.IsNullOrWhiteSpace(EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"))) {
          if (int.TryParse(EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"), out result)) {
            pullRequestKey = result;
          }
        }
        settings.PullRequestBase = EnvironmentVariable("APPVEYOR_REPO_BRANCH"); //sonar.pullrequest.base=master
        settings.PullRequestBranch = EnvironmentVariable("APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH");  //sonar.pullrequest.branch=feature/my-new-feature
        settings.PullRequestKey = pullRequestKey;//sonar.pullrequest.key=5
        settings.PullRequestProvider = EnvironmentVariable("APPVEYOR_REPO_PROVIDER"); //sonar.pullrequest.provider
        settings.PullRequestGithubRepository = EnvironmentVariable("APPVEYOR_REPO_NAME"); //sonar.pullrequest.github.repository=my-company/my-repo
      } else {
        settings.Branch = branch;
      }

      Sonar(ctx => ctx.DotNetCoreMSBuild("../TrekkingForCharity.AlgoliaLocal.sln"), settings);

      if (FileExists("./build-artifacts/test/opencover.xml")) {
        Codecov("./build-artifacts/test/opencover.xml", codecovToken);
      }
    }
  });

Teardown(context => {
  var files = GetFiles("../source/**/*.csproj");
  foreach (var file in files) {
    Information("Resetting version info for: " + file.ToString());
    XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", "1.0.0");
    XmlPoke(file, "/Project/PropertyGroup/FileVersion", "1.0.0");
    XmlPoke(file, "/Project/PropertyGroup/Version", "1.0.0");
  }
});

Task("Build")
  .IsDependentOn("__Clean")
  .IsDependentOn("__Versioning")
  .IsDependentOn("__NugetRestore")
  .IsDependentOn("__Test")
  .IsDependentOn("__Publish")
  .IsDependentOn("__Package")
  .IsDependentOn("__ProcessDataForThirdParties")
  ;

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);


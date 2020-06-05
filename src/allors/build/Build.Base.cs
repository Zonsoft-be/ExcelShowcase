using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;

partial class Build
{
    Target BaseResetDatabase => _ => _
        .Executes(() =>
        {
            var database = "Base";
            using (var sqlServer = new SqlServer())
            {
                sqlServer.Restart();
                sqlServer.Drop(database);
                sqlServer.Create(database);
            }
        });

    private Target BaseDatabaseTest => _ => _
         .DependsOn(BaseDatabaseTestDomain);

    private Target BaseMerge => _ => _
        .Executes(() =>
        {
            DotNetRun(s => s
                .SetProjectFile(Paths.CoreDatabaseMerge)
                .SetApplicationArguments($"{Paths.CoreDatabaseResourcesCore} {Paths.BaseDatabaseResourcesBase} {Paths.BaseDatabaseResources}"));
        });

    private Target BaseDatabaseTestDomain => _ => _
         .DependsOn(BaseGenerate)
         .Executes(() =>
         {
             DotNetTest(s => s
                 .SetProjectFile(Paths.BaseDatabaseDomainTests)
                 .SetLogger("trx;LogFileName=BaseDatabaseDomain.trx")
                 .SetResultsDirectory(Paths.ArtifactsTests));
         });

    private Target BaseGenerate => _ => _
         .After(Clean)
         .DependsOn(BaseMerge)
         .Executes(() =>
         {
             DotNetRun(s => s
                 .SetProjectFile(Paths.PlatformRepositoryGenerate)
                 .SetApplicationArguments($"{Paths.BaseRepositoryDomainRepository} {Paths.PlatformRepositoryTemplatesMetaCs} {Paths.BaseDatabaseMetaGenerated}"));
             DotNetRun(s => s
                 .SetWorkingDirectory(Paths.Base)
                 .SetProjectFile(Paths.BaseDatabaseGenerate));
         });

    private Target BasePublishCommands => _ => _
         .DependsOn(BaseGenerate)
         .Executes(() =>
         {
             var dotNetPublishSettings = new DotNetPublishSettings()
                 .SetWorkingDirectory(Paths.BaseDatabaseCommands)
                 .SetOutput(Paths.ArtifactsBaseCommands);
             DotNetPublish(dotNetPublishSettings);
         });

    private Target BasePublishServer => _ => _
             .DependsOn(BaseGenerate)
         .Executes(() =>
         {
             var dotNetPublishSettings = new DotNetPublishSettings()
                 .SetWorkingDirectory(Paths.BaseDatabaseServer)
                 .SetOutput(Paths.ArtifactsBaseServer);
             DotNetPublish(dotNetPublishSettings);
         });

    private Target BaseWorkspaceAutotest => _ => _
         .DependsOn(BaseWorkspaceSetup)
         .Executes(() =>
         {
             foreach (var path in new[] { Paths.BaseWorkspaceTypescriptIntranet, Paths.BaseWorkspaceTypescriptAutotestAngular })
             {
                 NpmRun(s => s
                     .SetEnvironmentVariable("npm_config_loglevel", "error")
                     .SetWorkingDirectory(path)
                     .SetCommand("autotest"));
             }

             DotNetRun(s => s
                 .SetWorkingDirectory(Paths.Base)
                 .SetProjectFile(Paths.BaseWorkspaceTypescriptAutotestGenerateGenerate));
         });

    private Target BaseWorkspaceNpmInstall => _ => _
                         .Executes(() =>
         {
             foreach (var path in Paths.BaseWorkspaceTypescript)
             {
                 NpmInstall(s => s
                     .SetEnvironmentVariable("npm_config_loglevel", "error")
                     .SetWorkingDirectory(path));
             }
         });

    private Target BaseWorkspaceSetup => _ => _
         .DependsOn(BaseWorkspaceNpmInstall)
         .DependsOn(BaseGenerate);

    private Target BaseWorkspaceTypescriptDomain => _ => _
         .DependsOn(BaseWorkspaceSetup)
         .DependsOn(EnsureDirectories)
         .Executes(() =>
         {
             NpmRun(s => s
                 .SetEnvironmentVariable("npm_config_loglevel", "error")
                 .SetWorkingDirectory(Paths.BaseWorkspaceTypescriptDomain)
                 .SetArguments("--reporter-options", $"output={Paths.ArtifactsTestsBaseWorkspaceTypescriptDomain}")
                 .SetCommand("az:test"));
         });

    private Target BaseWorkspaceTypescriptIntranet => _ => _
         .DependsOn(BaseWorkspaceSetup)
         .DependsOn(BasePublishServer)
         .DependsOn(BasePublishCommands)
         .DependsOn(BaseResetDatabase)
         .Executes(async () =>
         {
             using (var sqlServer = new SqlServer())
             {
                 sqlServer.Restart();
                 sqlServer.Populate(Paths.ArtifactsBaseCommands);

                 using (var server = new Server(Paths.ArtifactsBaseServer))
                 {
                     await server.Ready();
                     NpmRun(
                         s => s
                             .SetEnvironmentVariable("npm_config_loglevel", "error")
                             .SetWorkingDirectory(Paths.BaseWorkspaceTypescriptIntranet)
                             .SetArguments("--watch=false", "--reporters", "trx")
                             .SetCommand("test"));
                     CopyFileToDirectory(
                         Paths.BaseWorkspaceTypescriptIntranetTrx,
                         Paths.ArtifactsTests,
                         FileExistsPolicy.Overwrite);
                 }
             }
         });

    private Target BaseWorkspaceTypescriptIntranetGenericTests => _ => _
         .DependsOn(BaseWorkspaceAutotest)
         .DependsOn(BasePublishServer)
         .DependsOn(BasePublishCommands)
         .DependsOn(BaseResetDatabase)
         .Executes(async () =>
         {
             using (var sqlServer = new SqlServer())
             {
                 sqlServer.Restart();
                 sqlServer.Populate(Paths.ArtifactsBaseCommands);
                 using (var server = new Server(Paths.ArtifactsBaseServer))
                 {
                     using (var angular = new Angular(Paths.BaseWorkspaceTypescriptIntranet))
                     {
                         await server.Ready();
                         await angular.Init();
                         DotNetTest(
                             s => s
                                 .SetProjectFile(Paths.BaseWorkspaceTypescriptIntranetTests)
                                 .SetLogger("trx;LogFileName=BaseWorkspaceTypescriptIntranetTests.trx")
                                 .SetFilter("Category=Generic")
                                 .SetResultsDirectory(Paths.ArtifactsTests));
                     }
                 }
             }
         });

    private Target BaseWorkspaceTypescriptIntranetSpecificTests => _ => _
        .DependsOn(BaseWorkspaceAutotest)
        .DependsOn(BasePublishServer)
        .DependsOn(BasePublishCommands)
        .DependsOn(BaseResetDatabase)
        .Executes(async () =>
        {
            using (var sqlServer = new SqlServer())
            {
                sqlServer.Restart();
                sqlServer.Populate(Paths.ArtifactsBaseCommands);
                using (var server = new Server(Paths.ArtifactsBaseServer))
                {
                    using (var angular = new Angular(Paths.BaseWorkspaceTypescriptIntranet))
                    {
                        await server.Ready();
                        await angular.Init();
                        DotNetTest(
                            s => s
                                .SetProjectFile(Paths.BaseWorkspaceTypescriptIntranetTests)
                                .SetLogger("trx;LogFileName=BaseWorkspaceTypescriptIntranetTests.trx")
                                .SetFilter("Category!=Generic")
                                .SetResultsDirectory(Paths.ArtifactsTests));
                    }
                }
            }
        });

    private Target BaseWorkspaceTypescriptTest => _ => _
        .DependsOn(BaseWorkspaceTypescriptDomain)
        .DependsOn(BaseWorkspaceTypescriptIntranet);

    private Target BaseTest => _ => _
        .DependsOn(BaseDatabaseTest)
        .DependsOn(BaseWorkspaceTypescriptTest);

    private Target Base => _ => _
        .DependsOn(Clean)
        .DependsOn(BaseTest);
}

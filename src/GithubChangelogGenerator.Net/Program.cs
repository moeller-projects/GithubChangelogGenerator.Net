using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using GithubChangelogGenerator.Net.Extensions;
using GithubChangelogGenerator.Net.Parser;
using GithubChangelogGenerator.Net.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GithubChangelogGenerator.Net
{
    [Command(Name = "changelog", Description = "Generates a Changelog base on Github Commits/Issues/PR and so on")]
    public class Program : CommandBase
    {
        public const ConsoleColor NoStatus = ConsoleColor.Yellow;
        public const ConsoleColor Success = ConsoleColor.Green;
        public const ConsoleColor Failed = ConsoleColor.Red;
        
        private readonly ILogger<Program> _logger;
        private readonly IConsole _console;
        private readonly IGithubService _githubService;
        private readonly IChangelogParser _changelogParser;

        public Program(
            ILogger<Program> logger,
            IConsole console,
            IGithubService githubService,
            IChangelogParser changelogParser
        )
        {
            _logger = logger;
            _console = console;
            _githubService = githubService;
            _changelogParser = changelogParser;
        }

        private static IConfiguration BuildConfig(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        public static async Task<int> Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddDebug())
                .AddSingleton<IConfiguration>(BuildConfig(args))
                .AddSingleton<IConsole>(PhysicalConsole.Singleton)
                .AddSingleton<IFileSystem, FileSystem>()
                .AddMemoryCache()
                .AddSingleton<IChangelogParser, ChangelogParser>()
                .AddSingleton<IGithubService, GithubService>()
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);
            return await app.ExecuteAsync(args);
        }

        protected override async Task OnExecuteAsync()
        {
            var repositoryClient = _githubService.GetGithubClient().Repository;
            
            _console.Write($"Loading Repository {UserName}/{Repository}: ", NoStatus);
            var repository = await _githubService.GetUserRepository();
            _console.WriteLine("Done", Success);
            
            _console.Write("Loading Repository Commits: ", NoStatus);
            var commits = await _githubService.GetCommitsOfUserRepository();
            _console.WriteLine("Done", Success);
            
            _console.Write("Loading Repository Releases: ", NoStatus);
            var releases = (await _githubService.GetReleasesOfUserRepository())
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.PublishedAt ?? o.CreatedAt)
                .ToList().AsReadOnly();
            _console.WriteLine("Done", Success);
            
            _console.Write("Loading Repository Tags: ", NoStatus);
            var tags = await _githubService.GetTagsOfUserRepository();
            _console.WriteLine("Done", Success);

            _console.Write("Create Repository Changelog: ", NoStatus);
            var document = await _changelogParser.Parse(
                repository,
                commits,
                releases,
                tags);
            _console.WriteLine("Done", Success);

            _console.Write("Commit generated Changelog-File: ", NoStatus);
            await UpdateChangelogFileInRepository(repositoryClient, repository, document);
            _console.WriteLineEnter("Done", Success);
            _console.WriteLineEnter("Generated Changelog: ", NoStatus);
            _console.Write(document);
        }

        private async Task UpdateChangelogFileInRepository(IRepositoriesClient repositoriesClient, Repository repository, string document)
        {
            var existingChangelog = (await repositoriesClient.Content.GetAllContents(repository.Id))
                .FirstOrDefault(any => any.Name == "CHANGELOG.md");
            
            if (existingChangelog is null)
            {
                await repositoriesClient.Content.CreateFile(
                    repository.Id,
                    "CHANGELOG.md",
                    new CreateFileRequest("Added CHANGELOG.md",
                        document,
                        string.IsNullOrWhiteSpace(Branch)
                            ? repository.DefaultBranch
                            : Arguments.Arguments.Branch));
            }
            else
            {
                await repositoriesClient.Content.UpdateFile(
                    repository.Id,
                    "CHANGELOG.md",
                    new UpdateFileRequest("Updated CHANGELOG.md",
                        document,
                        existingChangelog.Sha,
                        string.IsNullOrWhiteSpace(Branch)
                            ? repository.DefaultBranch
                            : Arguments.Arguments.Branch));
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GithubChangelogGenerator.Net.Services
{
    public class GithubService : IGithubService
    {
        private readonly ILogger<GithubService> _logger;
        private readonly IConsole _console;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly GitHubClient _githubClient;

        public GithubService(ILogger<GithubService> logger, IConsole console, IConfiguration config, IMemoryCache cache)
        {
            _logger = logger;
            _console = console;
            _config = config;
            _cache = cache;
            _githubClient = new GitHubClient(new ProductHeaderValue(nameof(GithubChangelogGenerator)))
            {
                Credentials = new Credentials(_config["token"])
            };
        }

        public IGitHubClient GetGithubClient() => _githubClient;

        public async Task<Repository> GetUserRepository()
            => await _githubClient.Repository.Get(_config["username"], _config["repository"]);

        public async Task<IReadOnlyList<GitHubCommit>> GetCommitsOfUserRepositoryForSpecifiedBranch()
        {
            var repository = await GetUserRepository();
            return await _githubClient.Repository.Commit.GetAll(repository.Id, new CommitRequest
            {
                Sha = string.IsNullOrWhiteSpace(_config[nameof(Arguments.Arguments.Branch)])
                ? repository.DefaultBranch
                : _config[nameof(Arguments.Arguments.Branch)]
            });
        }

        public async Task<IReadOnlyList<Release>> GetReleasesOfUserRepository()
        {
            var repository = await GetUserRepository();
            return await _githubClient.Repository.Release.GetAll(repository.Id);
        }

        public async Task<IReadOnlyList<RepositoryTag>> GetTagsOfUserRepository()
        {
            var repository = await GetUserRepository();
            return await _githubClient.Repository.GetAllTags(repository.Id);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace GithubChangelogGenerator.Net.Services
{
    public interface IGithubService
    {
        public IGitHubClient GetGithubClient();
        public Task<Repository> GetUserRepository();
        public Task<IReadOnlyList<GitHubCommit>> GetCommitsOfUserRepositoryForSpecifiedBranch();
        public Task<IReadOnlyList<Release>> GetReleasesOfUserRepository();
        public Task<IReadOnlyList<RepositoryTag>> GetTagsOfUserRepository();
    }
}
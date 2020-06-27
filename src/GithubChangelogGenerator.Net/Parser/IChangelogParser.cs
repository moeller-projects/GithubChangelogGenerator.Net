using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace GithubChangelogGenerator.Net.Parser
{
    public interface IChangelogParser
    {
        Task<string> Parse(
            Repository repository,
            IReadOnlyCollection<GitHubCommit> commits,
            IReadOnlyCollection<Release> releases,
            IReadOnlyCollection<RepositoryTag> tags);

        Task<string> Parse(IEnumerable<GitHubCommit> commits);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GithubChangelogGenerator.Net.Extensions;
using GithubChangelogGenerator.Net.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GithubChangelogGenerator.Net.Parser
{
    public class ChangelogParser : IChangelogParser
    {
        private readonly ILogger<ChangelogParser> _logger;
        private readonly IConsole _console;
        private readonly IConfiguration _configuration;

        public ChangelogParser(
            ILogger<ChangelogParser> logger,
            IConsole console,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _console = console;
            _configuration = configuration;
        }
        
        public async Task<string> Parse(
            Repository repository,
            IReadOnlyCollection<GitHubCommit> commits,
            IReadOnlyCollection<Release> releases,
            IReadOnlyCollection<RepositoryTag> tags)
        {
            var document = new MarkdownDocument(Arguments.Arguments.Header);
            var unreleasedSection = await GenerateUnreleasedSection(
                commits,
                releases.Max(release => release.CreatedAt).UtcDateTime);
            document.AddSection(unreleasedSection);

            for (var releaseNumber = 0; releaseNumber < releases.Count(); releaseNumber++)
            {
                var release = releases.ElementAtOrDefault(releaseNumber);
                var previousRelease = releases.ElementAtOrDefault(releaseNumber + 1);

                var tag = tags.FirstOrDefault(f => f.Name == release.TagName);
                var previousTag = tags.FirstOrDefault(f => f.Name == previousRelease?.TagName);

                var tagCommit = commits.FirstOrDefault(f => f.Sha == tag.Commit.Sha);
                var previousTagCommit = commits.FirstOrDefault(f => f.Sha == previousTag?.Commit.Sha);

                var commitsInRelease = commits.WhereDateIsBetween(
                        commit => commit.Commit.Committer.Date.UtcDateTime,
                        previousTagCommit?.Commit.Committer.Date.UtcDateTime ?? DateTime.MinValue,
                        tagCommit.Commit.Committer.Date.UtcDateTime)
                    .OrderByDescending(commit => commit.Commit.Committer.Date.UtcDateTime).ToArray();

                var changes = await Parse(commitsInRelease);
                var releaseSection = $"## [{release.TagName}] - {(release.CreatedAt).UtcDateTime:yyyy-mm-dd}" +
                                     Environment.NewLine;
                document.AddSection(releaseSection + changes);
            }

            document.AddSection(GenerateLinkSection(releases, repository));

            return document.ToString();
        }

        public async Task<string> Parse(IEnumerable<GitHubCommit> commits)
        {
            commits = commits.ToArray();
            (string Label, List<string> Aliases)[] changelogSections =
            {
                (Program.FixedSectionLabel, Program.FixedAliases),
                (Program.AddedSectionLabel, Program.AddedAliases),
                (Program.ChangedSectionLabel, Program.ChangedAliases),
                (Program.RemovedSectionLabel, Program.RemovedAliases),
            };

            var builder = new StringBuilder();
            foreach (var (label, aliases) in changelogSections)
            {
                var matchingCommits = commits.Where(commit =>
                {
                    var message = commit.Commit.Message?.Trim();
                    return !(message is null)
                           && aliases.Any(alias =>
                               message.StartsWith(alias, StringComparison.OrdinalIgnoreCase)
                               || message.StartsWith($"[{alias}]", StringComparison.OrdinalIgnoreCase));
                }).ToArray();

                if (!matchingCommits.Any())
                    continue;

                builder.AppendLine($"### {label}");
                var changes = matchingCommits
                    .GroupBy(commit => commit.Commit.Message)
                    .Select(commit => commit.First())
                    .Aggregate(string.Empty,
                        (changes, commit) =>
                            changes += $"- {commit.Commit.Message} " +
                                       $"([{commit.Sha.Substring(0, 7)}]({commit.HtmlUrl}))" +
                                       Environment.NewLine);
                builder.AppendLine(changes);
                builder.AppendLine();
            }
            
            var contributors = commits.SelectMany(commit =>
                {
                    return new (string? Name, string? Url)[]
                    {
                        (commit.Author?.Login, commit.Author?.HtmlUrl),
                        (commit.Committer?.Login, commit.Committer?.HtmlUrl),
                    };
                })
                .Where(user => !string.IsNullOrWhiteSpace(user.Name) && !string.IsNullOrWhiteSpace(user.Url))
                .Distinct()
                .Aggregate("Contributors:", (section, user) => section += $" [{user.Name}]({user.Url}),")
                .TrimEnd(',');
            
            builder.AppendLine(contributors);
            builder.AppendLine();

            return await Task.FromResult(builder.ToString());
        }

        
        private static string GenerateLinkSection(IReadOnlyCollection<Release> releases, Repository repository)
        {
            var builder = new StringBuilder();
            var previousReleaseTag = releases
                .OrderByDescending(release => release.CreatedAt)
                .FirstOrDefault()?.TagName;
            builder.AppendLine($"[{Arguments.Arguments.UnreleasedSectionLabel}]: " +
                               $"https://github.com/{repository.Owner.Login}/{repository.Name}/compare/{previousReleaseTag}...HEAD");

            for (var releaseNumber = 0; releaseNumber < releases.Count(); releaseNumber++)
            {
                var release = releases.ElementAtOrDefault(releaseNumber);
                var previousRelease = releases.ElementAtOrDefault(releaseNumber + 1);

                if (previousRelease is null)
                {
                    builder.AppendLine($"[{release.TagName}]: " +
                                       $"https://github.com/{repository.Owner.Login}/{repository.Name}/releases/tag/{release.TagName}");
                    continue;
                }

                builder.AppendLine($"[{release.TagName}]: " +
                                   $"https://github.com/{repository.Owner.Login}/{repository.Name}/compare/{previousRelease.TagName}...{release.TagName}");
            }

            return builder.ToString();
        }

        private async Task<string> GenerateUnreleasedSection(
            IEnumerable<GitHubCommit> commits,
            DateTime lastRelease)
        {
            var unreleasedCommits = commits.WhereDateIsBetween(
                    commit => commit.Commit.Committer.Date.UtcDateTime,
                    lastRelease,
                    DateTime.MaxValue)
                .OrderByDescending(commit => commit.Commit.Committer.Date.UtcDateTime).ToArray();

            var unreleasedSection = await Parse(unreleasedCommits);

            return $"## [{Arguments.Arguments.UnreleasedSectionLabel}]" +
                   Environment.NewLine +
                   unreleasedSection;
        }
    }
}
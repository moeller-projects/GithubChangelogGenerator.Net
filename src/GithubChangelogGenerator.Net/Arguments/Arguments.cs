using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace GithubChangelogGenerator.Net.Arguments
{
    public abstract class Arguments
    {
        #region GithubArguments

        [Required]
        [Option(
            CommandOptionType.SingleValue,
            Description = "Github PersonalAccessToken",
            ShortName = "t", LongName = "token", ValueName = "TOKEN")]
        public string? AuthToken { get; } = null!;

        [Required]
        // [Argument(1,
        //     Description = "Github Username",
        //     Name = "USERNAME",
        //     ShowInHelpText = true)]
        [Option(
            CommandOptionType.SingleValue,
            Description = "Github UserName",
            ShortName = "u", LongName = "username", ValueName = "USERNAME")]
        public string? UserName { get; } = null!;

        [Required]
        // [Argument(2,
        //     Description = "Github Repository Name",
        //     Name = "REPOSITORY",
        //     ShowInHelpText = true)]
        [Option(
            CommandOptionType.SingleValue,
            Description = "Github Repository",
            ShortName = "r", LongName = "repository", ValueName = "REPOSITORY")]
        public string? Repository { get; } = null!;
        
        [Option(
            CommandOptionType.SingleValue,
            Description = "Github Repository Branch",
            ShortName = "b", LongName = "branch", ValueName = "BRANCH")]
        public static string? Branch { get; } = null!;

        #endregion

        #region ChangelogArguments
        
        [Option(CommandOptionType.SingleValue, Description = "Override the Default Changelog Header",
            ShortName = "", LongName = "header", ValueName = "HEADER")]
        public static string Header { get; } = "Changelog";

        [Option(CommandOptionType.SingleValue, Description = "Override the Default Unreleased-Section Label",
            ShortName = "", LongName = "unreleased-label", ValueName = "LABEL")]
        public static string UnreleasedSectionLabel { get; } = "Unreleased";

        [Option(CommandOptionType.SingleValue, Description = "Override the Default Added-Section Label",
            ShortName = "", LongName = "added-label", ValueName = "LABEL")]
        public static string AddedSectionLabel { get; } = "Added";

        [Option(CommandOptionType.MultipleValue, Description = "Override the Default Added-Section Aliases",
            ShortName = "", LongName = "added-alias", ValueName = "ALIAS")]
        public static List<string> AddedAliases { get; } = new List<string>()
            {"Add", "Added", "Create", "Created", "Implement", "Implemented"};

        [Option(CommandOptionType.SingleValue, Description = "Override the Default Removed-Section Label",
            ShortName = "", LongName = "removed-label", ValueName = "LABEL")]
        public static string RemovedSectionLabel { get; } = "Removed";

        [Option(CommandOptionType.MultipleValue, Description = "Override the Default Removed-Section Aliases",
            ShortName = "", LongName = "removed-alias", ValueName = "ALIAS")]
        public static List<string> RemovedAliases { get; } = new List<string>() {"Remove", "Removed", "Delete", "Deleted"};

        [Option(CommandOptionType.SingleValue, Description = "Override the Default Changed-Section Label",
            ShortName = "", LongName = "changed-label", ValueName = "LABEL")]
        public static string ChangedSectionLabel { get; } = "Changed";

        [Option(CommandOptionType.MultipleValue, Description = "Override the Default Changed-Section Aliases",
            ShortName = "", LongName = "changed-alias", ValueName = "ALIAS")]
        public static List<string> ChangedAliases { get; } = new List<string>() {"Change", "Changed", "Update", "Updated"};

        [Option(CommandOptionType.SingleValue, Description = "Override the Default Fixed-Section Label",
            ShortName = "", LongName = "fixed-label", ValueName = "LABEL")]
        public static string FixedSectionLabel { get; } = "Fixed";

        [Option(CommandOptionType.MultipleValue, Description = "Override the Default Fixed-Section Aliases",
            ShortName = "", LongName = "fixed-alias", ValueName = "ALIAS")]
        public static List<string> FixedAliases { get; } = new List<string>() {"Fix", "Fixed", "Hotfix", "Hotfixed", "Bug"};

        #endregion
    }
}
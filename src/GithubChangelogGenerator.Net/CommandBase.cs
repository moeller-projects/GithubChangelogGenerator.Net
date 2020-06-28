using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GithubChangelogGenerator.Net
{
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    public abstract class CommandBase : Arguments.Arguments
    {
        public static string? GetVersion() => Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        protected abstract Task OnExecuteAsync();
    }
}
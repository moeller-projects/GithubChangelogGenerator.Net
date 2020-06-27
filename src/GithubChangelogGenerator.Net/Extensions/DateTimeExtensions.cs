using System;

namespace GithubChangelogGenerator.Net.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool Between(this DateTime instant, DateTime from, DateTime to)
        {
            if (@from > to) throw new ArgumentException($"{nameof(@from)} may not be after {nameof(to)}", nameof(@from));
            return instant >= @from && instant <= to;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace GithubChangelogGenerator.Net.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereDateIsBetween<T>(
            this IEnumerable<T> enumerable,
            Func<T, DateTime> dateTimeSelector,
            DateTime from,
            DateTime to
            )
        {
            if (from > to) throw new ArgumentException($"{nameof(from)} may not be after {nameof(to)}", nameof(from));
            return enumerable
                .Where(entity => dateTimeSelector(entity).Between(@from, to));
        }
    }
}
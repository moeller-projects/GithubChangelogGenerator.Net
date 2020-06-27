using System;
using System.Collections.Generic;
using System.Text;

namespace GithubChangelogGenerator.Net.Models
{
    public class MarkdownDocument
    {
        public string? Header { get; private set; }
        public List<string> Sections { get; set; }

        public MarkdownDocument(string header)
        {
            Header = header?.Trim();
            Sections = new List<string>();
        }

        public MarkdownDocument SetHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header))
                throw new ArgumentNullException(nameof(header), $"{nameof(header)} should not be null or whitespace");
            
            Header = header;
            return this;
        }

        public MarkdownDocument AddSection(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentNullException(nameof(section), $"{nameof(section)} should not be null or whitespace");
            
            Sections.Add(section);
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"# {Header}");
            builder.AppendLine();
            foreach (var section in Sections)
                builder.AppendLine(section);
            return builder.ToString();
        }
    }
}
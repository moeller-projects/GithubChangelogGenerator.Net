﻿using System;
using McMaster.Extensions.CommandLineUtils;

namespace GithubChangelogGenerator.Net.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WriteLine(this IConsole console, string value, ConsoleColor foregroundColor)
        {
            var currentColor = console.ForegroundColor;

            console.ForegroundColor = foregroundColor;
            console.WriteLine(value);
            console.ForegroundColor = currentColor;
        }

        public static void Write(this IConsole console, string value, ConsoleColor foregroundColor)
        {
            var currentColor = console.ForegroundColor;

            console.ForegroundColor = foregroundColor;
            console.Write(value);
            console.ForegroundColor = currentColor;
        }

        public static void Write(this IConsole console, string value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            var currentForegroundColor = console.ForegroundColor;
            var currentBackgroundColor = console.BackgroundColor;

            console.ForegroundColor = foregroundColor;
            console.BackgroundColor = backgroundColor;
            console.Write(value);
            console.ForegroundColor = currentForegroundColor;
            console.BackgroundColor = currentBackgroundColor;
        }

        public static void WriteIndent(this IConsole console, int level)
        {
            console.Write(new string(' ', level * 2));
        }

        public static void WriteEnter(this IConsole console, int level = 1)
        {
            for (var enter = 0; enter < level; enter++) console.WriteLine();
        }

        public static void WriteLineEnter(this IConsole console, string value, int level = 1)
        {
            console.WriteLine(value);
            for (var enter = 0; enter < level; enter++) console.WriteLine();
        }
        
        public static void WriteLineEnter(this IConsole console, string value, ConsoleColor foregroundColor, int level = 1)
        {
            console.WriteLine(value, foregroundColor);
            for (var enter = 0; enter < level; enter++) console.WriteLine();
        }
    }
}
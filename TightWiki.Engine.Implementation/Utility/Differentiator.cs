﻿using System.Text;

namespace TightWiki.Engine.Implementation.Utility
{
    public static class Differentiator
    {
        /// <summary>
        /// This leaves a lot to be desired.
        /// </summary>
        public static string GetComparisonSummary(string thisRev, string prevRev)
        {
            var summary = new StringBuilder();

            var thisRevLines = thisRev.Split('\n');
            var prevRevLines = prevRev.Split('\n');
            int thisRevLineCount = thisRevLines.Length;
            int prevRevLinesCount = prevRevLines.Length;

            int linesAdded = prevRevLines.Except(thisRevLines).Count();
            int linesDeleted = thisRevLines.Except(prevRevLines).Count();

            if (thisRevLineCount != prevRevLinesCount)
            {
                summary.Append($"{Math.Abs(thisRevLineCount - prevRevLinesCount):N0} lines changed.");
            }

            if (linesAdded > 0)
            {
                if (summary.Length > 0) summary.Append(' ');
                summary.Append($"{linesAdded:N0} lines added.");
            }

            if (linesDeleted > 0)
            {
                if (summary.Length > 0) summary.Append(' ');
                summary.Append($"{linesDeleted:N0} lines deleted.");
            }

            if (summary.Length == 0)
            {
                summary.Append($"No changes detected.");
            }

            return summary.ToString();
        }
    }
}

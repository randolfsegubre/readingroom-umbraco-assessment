namespace ReadingRoom.ArticleCMS.Core.Takeaways;

public static class TakeawayLines
{
    /// <summary>
    /// Returns the takeaways in their original order, without empty lines.
    /// </summary>
    /// <param name="lines">The lines saved by the editor. Null gives an empty list.</param>
    public static List<string> WithoutBlanks(IEnumerable<string>? lines)
    {
        List<string> takeaways = new List<string>();
        foreach (string line in lines ?? Enumerable.Empty<string>())
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                takeaways.Add(line);
            }
        }

        return takeaways;
    }
}

namespace ReadingRoom.ArticleCMS.Core.Articles;

/// <summary>
/// A reading level an editor can pick for an article.
/// </summary>
/// <param name="Value">The text that is saved with the article.</param>
/// <param name="DictionaryKey">The dictionary item with the translated name.</param>
/// <param name="EnglishName">The name shown when the dictionary item is missing.</param>
public sealed record ReadingLevel(string Value, string DictionaryKey, string EnglishName);

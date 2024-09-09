namespace Support.Translation
{
    /// <summary>
    /// Allow object to be translated using a language pack. Must have its own language key.
    /// </summary>
    internal interface ILanguageTranslatable
    {
        void Translate(LanguagePack languagePack);
    }
}
using System.Collections.Generic;
using Godot;
using Support.Diagnostics;
using GodotDictionaryKeyString = Godot.Collections.Dictionary<string, string>;

namespace Support.Translation;

public partial class LanguagePack : Resource
{
    [Export] private GodotDictionaryKeyString? entries;
    public const string minuteLanguageKey = "ui.time.minute";
    public const string minutesLanguageKey = "ui.time.minutes";
    public const string secondLanguageKey = "ui.time.second";
    public const string secondsLanguageKey = "ui.time.seconds";
    public const string andLanguageKey = "ui.and";
    private IReadOnlyDictionary<string, string>? entriesDictionary;
    public string Translate(string key) => entriesDictionary![key];
    public bool CanTranslate(string key)
    {
        Debug.Assert(entriesDictionary != null, $"'dictEntries' was not initialized for this language pack ({ResourceName}), consider calling Setup.");
        Debug.Assert(entriesDictionary!.ContainsKey(key), $"Key '{key}' was not present in this language pack ({ResourceName}).");
        LogManager.Instance!.Log("Translation", $"Received translation request with the key '{key}' not present in language pack ({ResourceName}).");
        return entriesDictionary.ContainsKey(key);
    }
    public void ApplyLanguagePackOnNode(in Node node)
    {
        //TODO: implement when use
        entriesDictionary = new Dictionary<string, string>();
        // Get Component with ILanguageTranslatable.Translate(this);
        throw new System.NotImplementedException();
    }
    public static string SuggestLanguageTag()
    {
        return OS.GetLocaleLanguage();
    }
}
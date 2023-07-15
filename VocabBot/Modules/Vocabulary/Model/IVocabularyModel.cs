namespace VocabBot.Modules.Vocabulary.Model;

public interface IVocabularyModel
{
    Dictionary<string, List<string>> Dictionary { get; }

    bool TryInitialize();
}
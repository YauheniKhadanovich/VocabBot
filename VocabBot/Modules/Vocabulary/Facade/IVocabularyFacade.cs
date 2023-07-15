namespace VocabBot.Modules.Vocabulary.Facade;

public interface IVocabularyFacade
{
    Dictionary<string, List<string>> Dictionary { get; }
    bool TryInitialize();
    KeyValuePair<string, List<string>> GetRandomQuestion();
}
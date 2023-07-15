namespace VocabBot.Modules.Vocabulary.Facade;

public interface IVocabularyFacade
{
    bool TryInitialize();
    KeyValuePair<string, List<string>> GetRandomQuestion();
    bool IsCorrectAnswer(string question, string answer);
}
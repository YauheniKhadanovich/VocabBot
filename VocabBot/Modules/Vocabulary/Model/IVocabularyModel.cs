namespace VocabBot.Modules.Vocabulary.Model;

public interface IVocabularyModel
{
    bool TryInitialize();
    KeyValuePair<string, List<string>> GetRandomQuestion();
    bool IsCorrectAnswer(string question, string answer);
}
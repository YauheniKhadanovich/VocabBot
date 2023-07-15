using VocabBot.Modules.Vocabulary.Model;
using VocabBot.Modules.Vocabulary.Model.Impl;

namespace VocabBot.Modules.Vocabulary.Facade.Impl;

public class VocabularyFacade : IVocabularyFacade
{
    private IVocabularyModel _vocabularyModel;
    
    // TODO: DI
    public VocabularyFacade()
    {
        _vocabularyModel = new VocabularyModel();
    }


    public bool TryInitialize()
    {
        return _vocabularyModel.TryInitialize();
    }

    public KeyValuePair<string, List<string>> GetRandomQuestion()
    {
        return _vocabularyModel.GetRandomQuestion();
    }

    public bool IsCorrectAnswer(string question, string answer)
    {
        return _vocabularyModel.IsCorrectAnswer(question, answer);
    }
}
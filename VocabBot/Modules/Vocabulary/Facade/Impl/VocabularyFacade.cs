using VocabBot.Modules.Vocabulary.Model;
using VocabBot.Modules.Vocabulary.Model.Impl;

namespace VocabBot.Modules.Vocabulary.Facade.Impl;

public class VocabularyFacade : IVocabularyFacade
{
    private IVocabularyModel _vocabularyModel;

    public Dictionary<string, List<string>> Dictionary => _vocabularyModel.Dictionary;
    
    // TODO: DI
    public VocabularyFacade()
    {
        _vocabularyModel = new VocabularyModel();
    }


    public bool TryInitialize()
    {
        return _vocabularyModel.TryInitialize();
    }
}
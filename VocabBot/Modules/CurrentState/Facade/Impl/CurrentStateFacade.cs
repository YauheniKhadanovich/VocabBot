using VocabBot.Modules.CurrentState.Model;
using VocabBot.Modules.CurrentState.Model.Data;
using VocabBot.Modules.CurrentState.Model.Data.Impl;
using VocabBot.Modules.CurrentState.Model.Impl;

namespace VocabBot.Modules.CurrentState.Facade.Impl;

public class CurrentStateFacade : ICurrentStateFacade
{
    public event Action<long> BotStarted = delegate { };

    private ICurrentStateModel _currentStateModel;
    
    // TODO: DI
    public CurrentStateFacade()
    {
        _currentStateModel = new CurrentStateModel();
        _currentStateModel.BotStarted += OnBotStarted;
    }

    public bool IsStarted(long chatId)
    {
        return _currentStateModel.IsStarted(chatId);
    }

    public void StartBot(long chatId)
    {
        _currentStateModel.StartBot(chatId);
    }

    public void WaitForAnswer(long chatId, string askedWord, long answeredMessageId)
    {
        _currentStateModel.WaitForAnswer(chatId, askedWord, answeredMessageId);
    }

    public void StopBot(long chatId)
    {
        _currentStateModel.StopBot(chatId);
    }

    public BotInChatState GetCurrentState(long chatId)
    {
        return _currentStateModel.GetCurrentState(chatId);
    }

    private void OnBotStarted(long chatId)
    {
        BotStarted.Invoke(chatId);
    }
}
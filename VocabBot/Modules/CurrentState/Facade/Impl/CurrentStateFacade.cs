using VocabBot.Modules.CurrentState.Model;
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

    public bool IsStarted => _currentStateModel.IsStarted;
    
    public void StartBot(long chatId)
    {
        _currentStateModel.StartBot(chatId);
    }

    public void StopBot(long chatId)
    {
        _currentStateModel.StopBot(chatId);
    }
    
    private void OnBotStarted(long chatId)
    {
        BotStarted.Invoke(chatId);
    }
}
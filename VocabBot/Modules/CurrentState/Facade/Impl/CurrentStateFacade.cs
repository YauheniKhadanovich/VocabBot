using VocabBot.Modules.CurrentState.Model;
using VocabBot.Modules.CurrentState.Model.Impl;

namespace VocabBot.Modules.CurrentState.Facade.Impl;

public class CurrentStateFacade : ICurrentStateFacade
{
    public event Action BotStarted = delegate { };

    private ICurrentStateModel _currentStateModel;
    // TODO: DI
    public CurrentStateFacade()
    {
        _currentStateModel = new CurrentStateModelModel();
        _currentStateModel.BotStarted += OnBotStarted;
    }

    public bool IsStarted => _currentStateModel.IsStarted;
    
    public void StartBot()
    {
        _currentStateModel.StartBot();
    }

    public void StopBot()
    {
        _currentStateModel.StopBot();
    }
    
    private void OnBotStarted()
    {
        BotStarted.Invoke();
    }
}
namespace VocabBot.Modules.CurrentState.Model.Impl;

public class CurrentStateModelModel : ICurrentStateModel
{
    public event Action BotStarted = delegate { };

    public bool IsStarted { get; private set; }

    public void StartBot()
    {
        IsStarted = true;
        BotStarted.Invoke();
    }

    public void StopBot()
    {
        IsStarted = false;
    }
}
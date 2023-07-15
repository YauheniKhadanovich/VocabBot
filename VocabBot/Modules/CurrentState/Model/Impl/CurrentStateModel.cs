namespace VocabBot.Modules.CurrentState.Model.Impl;

public class CurrentStateModel : ICurrentStateModel
{
    public event Action<long> BotStarted = delegate { };

    public bool IsStarted { get; private set; }

    public void StartBot(long chatId)
    {
        IsStarted = true;
        BotStarted.Invoke(chatId);
    }

    public void StopBot(long chatId)
    {
        IsStarted = false;
    }
}
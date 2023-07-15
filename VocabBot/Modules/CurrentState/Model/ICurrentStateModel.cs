namespace VocabBot.Modules.CurrentState.Model;

public interface ICurrentStateModel
{
    event Action<long> BotStarted;
    bool IsStarted { get; }

    void StartBot(long chatId);
    void StopBot(long chatId);
}
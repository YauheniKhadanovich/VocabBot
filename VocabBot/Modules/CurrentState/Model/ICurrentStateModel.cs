namespace VocabBot.Modules.CurrentState.Model;

public interface ICurrentStateModel
{
    event Action BotStarted;
    bool IsStarted { get; }

    void StartBot();
    void StopBot();
}
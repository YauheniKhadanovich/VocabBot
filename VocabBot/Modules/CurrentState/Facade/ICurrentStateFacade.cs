namespace VocabBot.Modules.CurrentState.Facade;

public interface ICurrentStateFacade
{
    event Action BotStarted;
    bool IsStarted { get; }
    
    void StartBot();
    void StopBot();
}
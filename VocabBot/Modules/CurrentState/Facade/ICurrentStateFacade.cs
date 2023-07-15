namespace VocabBot.Modules.CurrentState.Facade;

public interface ICurrentStateFacade
{
    event Action<long> BotStarted;
    bool IsStarted { get; }
    
    void StartBot(long chatId);
    void StopBot(long chatId);
}
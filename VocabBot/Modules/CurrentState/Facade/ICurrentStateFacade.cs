using VocabBot.Modules.CurrentState.Model.Data.Impl;

namespace VocabBot.Modules.CurrentState.Facade;

public interface ICurrentStateFacade
{
    event Action<long> BotStarted;

    bool IsStarted(long chatId);
    void StartBot(long chatId);
    void WaitForAnswer(long chatId, string askedWord, long answeredMessageId);
    void StopBot(long chatId);
    BotInChatState GetCurrentState(long chatId);
}
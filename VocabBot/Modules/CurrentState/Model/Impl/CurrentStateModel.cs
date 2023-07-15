using VocabBot.Modules.CurrentState.Model.Data;
using VocabBot.Modules.CurrentState.Model.Data.Impl;

namespace VocabBot.Modules.CurrentState.Model.Impl;

public class CurrentStateModel : ICurrentStateModel
{
    public event Action<long> BotStarted = delegate { };
    
    private Dictionary<long, BotInChatState> _states = new();

    public bool IsStarted(long chatId)
    {
        return _states.ContainsKey(chatId) && _states[chatId].State != State.Finished;
    }
    
    public void StartBot(long chatId)
    {
        SetCurrentState(chatId,State.Started);
        BotStarted.Invoke(chatId);
    }

    public void WaitForAnswer(long chatId, string askedWord, long answeredMessageId)
    {
        SetCurrentState(chatId,State.WaitingForAnswer, askedWord, answeredMessageId);
    }

    public void StopBot(long chatId)
    {
        SetCurrentState(chatId,State.Finished);
    }

    public BotInChatState GetCurrentState(long chatId)
    {
        return _states.TryGetValue(chatId, out var state) ? state : new BotInChatState();
    }
    
    private void SetCurrentState(long chatId, State newState)
    {
        if (_states.TryGetValue(chatId, out var state))
        {
            _states[chatId] = new BotInChatState(newState, state.AskedWord, state.AnsweredMessageId);
        }
        
        _states[chatId] = new BotInChatState(newState);
    }
    
    private void SetCurrentState(long chatId, State newState, string askedWord, long answeredMessageId)
    {
        _states[chatId] = new BotInChatState(newState, askedWord, answeredMessageId);
    }
}
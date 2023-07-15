namespace VocabBot.Modules.CurrentState.Model.Data.Impl;

public struct BotInChatState : IBotInChatState
{
    public State State { get; } = State.Finished;
    public string AskedWord { get; } = "";
    public long AnsweredMessageId { get; } = 0;
    
    public BotInChatState()
    {
        State = State.Finished;
        AskedWord = "";
        AnsweredMessageId = 0;
    }

    public BotInChatState(State state)
    {
        State = state;
        AskedWord = "";
        AnsweredMessageId = 0;
    }
    
    public BotInChatState(State state, string askedWord, long answeredMessageId)
    {
        State = state;
        AskedWord = askedWord;
        AnsweredMessageId = answeredMessageId;
    }
}
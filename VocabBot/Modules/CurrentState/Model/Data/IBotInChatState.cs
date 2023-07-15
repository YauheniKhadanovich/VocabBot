namespace VocabBot.Modules.CurrentState.Model.Data;

public interface IBotInChatState
{
    State State { get; }
    string AskedWord { get; }
}
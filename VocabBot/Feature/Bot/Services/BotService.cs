using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VocabBot.Core;
using VocabBot.Feature.Bot.Services.Impl;
using VocabBot.Modules.CurrentState.Facade;
using VocabBot.Modules.CurrentState.Facade.Impl;
using VocabBot.Modules.Vocabulary.Facade;
using VocabBot.Modules.Vocabulary.Facade.Impl;

namespace VocabBot.Feature.Bot.Services;

public class BotService : IBotService
{
    private ICurrentStateFacade _currentStateFacade = new CurrentStateFacade();
    private IVocabularyFacade _vocabularyFacade = new VocabularyFacade();
    private TelegramBotClient _bot = null!;

    public void Initialize()
    {
        Console.WriteLine("Initializing ...");

        _bot = new TelegramBotClient(Const.BotToken);
        using CancellationTokenSource cts = new();

        _bot.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            cancellationToken: cts.Token
        );

        if (!_vocabularyFacade.TryInitialize())
        {
            Console.WriteLine("Can't initialize model");
        }

        _currentStateFacade.BotStarted += OnBotStarted;
        Console.WriteLine("Initialization done!");
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null || update.Message?.Text.Length == 0)
        {
            return;
        }

        var chatId = update.Message.Chat.Id;
        var originMessage = update.Message;
        var userName = update.Message.From == null ? "<null user>" : update.Message.From.Username ?? "<empty userName>";
        var isCorrectMessage = update.Message is { Type: MessageType.Text, Text: not null };

        if (!isCorrectMessage)
        {
            Console.WriteLine($"{userName} : incorrect message.");
            return;
        }

        if (IsCommand(originMessage))
        {
            if (await TryExecuteCommand(originMessage))
            {
                Console.WriteLine($"{userName} : command {originMessage.Text} executed.");
                return;
            }
        }
        else
        {
            if (!_currentStateFacade.IsStarted)
            {
                Console.WriteLine("Bot disabled.");
                return;
            }

            var currentWord = _vocabularyFacade.Dictionary.Keys.ToArray()[new Random().Next(0, _vocabularyFacade.Dictionary.Count)];
            await botClient.SendTextMessageAsync(chatId, currentWord, cancellationToken: cancellationToken);
        }
    }

    async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
    }

    private bool IsCommand(Message message)
    {
        return message.Text![0] == '/';
    }

    async Task<bool> TryExecuteCommand(Message message)
    {
        switch (message.Text.ToLowerInvariant())
        {
            case Const.Commands.StartBot:
                if (_currentStateFacade.IsStarted)
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id, "I've already started =)");
                    return false;
                }

                _currentStateFacade.StartBot();
                await _bot.SendTextMessageAsync(message.Chat.Id, "Let's do it!");
                return true;
            case Const.Commands.StopBot:
                if (!_currentStateFacade.IsStarted)
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id, "I've already stopped =)");
                    return false;
                }

                _currentStateFacade.StopBot();
                await _bot.SendTextMessageAsync(message.Chat.Id, "Bye!");
                return true;
            default:
                await _bot.SendTextMessageAsync(message.Chat.Id, "Unsupported command");
                return false;
        }
    }

    private void OnBotStarted()
    {
    }
}
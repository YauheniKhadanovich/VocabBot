using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
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

        await OnMessageReceived(update.Message!);
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
        switch (message.Text!.ToLowerInvariant())
        {
            case Const.Commands.StartBot:
                if (_currentStateFacade.IsStarted)
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id, "I've already started =)");
                    return false;
                }

                _currentStateFacade.StartBot(message.Chat.Id);
                return true;
            case Const.Commands.StopBot:
                if (!_currentStateFacade.IsStarted)
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id, "I've already stopped =)");
                    return false;
                }

                _currentStateFacade.StopBot(message.Chat.Id);
                return true;
            default:
                await _bot.SendTextMessageAsync(message.Chat.Id, "Unsupported command");
                return false;
        }
    }

    private void OnBotStarted(long chatId)
    {
        var currentWord = _vocabularyFacade.Dictionary.Keys.ToArray()[new Random().Next(0, _vocabularyFacade.Dictionary.Count)];
        var task = _bot.SendTextMessageAsync(chatId, currentWord);
        task.Wait();
        var taskNextWord = AskNextWord(chatId);
        taskNextWord.Wait();
    }

    private async Task OnMessageReceived(Message message)
    {
        var userName = message.From == null ? "<null user>" : message.From.Username ?? "<empty userName>";
        var isCorrectMessage = message is { Type: MessageType.Text, Text: not null };

        if (!isCorrectMessage)
        {
            Console.WriteLine($"{userName} : incorrect message.");
            return;
        }

        if (IsCommand(message))
        {
            if (await TryExecuteCommand(message))
            {
                Console.WriteLine($"{userName} : command {message.Text} executed.");
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

            await AskNextWord(message.Chat.Id);
        }
    }

    private async Task AskNextWord(long chatId)
    {
        var question = _vocabularyFacade.GetRandomQuestion();
        var answers = question.Value; 
        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[] { answers[0] },
            new KeyboardButton[] { answers[1] },
            new KeyboardButton[] { answers[2] },
            new KeyboardButton[] { answers[3] },
        })
        {
            ResizeKeyboard = true
        };

        await _bot.SendTextMessageAsync(
            chatId: chatId,
            text: $"{question.Key}",
            replyMarkup: replyKeyboardMarkup);
    }
}
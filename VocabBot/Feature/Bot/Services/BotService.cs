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

    private void OnBotStarted(long chatId)
    {
        var taskNextWord = AskNextWord(chatId);
        taskNextWord.Wait();
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.CallbackQuery:
                if (update.CallbackQuery is { Data: not null, Message: not null })
                {
                    await OnCallbackQueryReceived(update.CallbackQuery!);
                }

                break;
            case UpdateType.Message:
                if (update.Message is { Text.Length: > 0 })
                {
                    await OnMessageReceived(update.Message!);
                }

                break;
        }
    }

    async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
    }

    async Task<bool> TryExecuteCommand(string userName, string text, long chatId)
    {
        var commandText = text.ToLowerInvariant();
        if (!Const.Commands.IsCommand(commandText))
        {
            return false;
        }
        
        Console.WriteLine($"chat: {chatId} user: {userName} : command {commandText}.");
        
        switch (commandText)
        {
            case Const.Commands.StartBot:
                if (_currentStateFacade.IsStarted)
                {
                    await _bot.SendTextMessageAsync(chatId, "I've already started =)");
                    return false;
                }

                _currentStateFacade.StartBot(chatId);
                return true;
            case Const.Commands.StopBot:
                if (!_currentStateFacade.IsStarted)
                {
                    await _bot.SendTextMessageAsync(chatId, "I've already stopped =)");
                    return false;
                }

                _currentStateFacade.StopBot(chatId);
                return true;
            default:
                await _bot.SendTextMessageAsync(chatId, "Unsupported command");
                return false;
        }
    }

    private async Task OnMessageReceived(Message message)
    {
        var userName = message.From == null ? "<null user>" : message.From.Username ?? "<empty userName>";

        await ProcessMessage(userName, message.Text!, message.Chat.Id);
    }

    private async Task OnCallbackQueryReceived(CallbackQuery callbackQuery)
    {
        var userName = callbackQuery.From.Username ?? "<empty userName>";

        await ProcessMessage(userName, callbackQuery.Data!, callbackQuery.Message!.Chat.Id);
    }

    private async Task ProcessMessage(string userName, string text, long chatId)
    {
        if (await TryExecuteCommand(userName, text, chatId))
        {
            return;
        }

        if (!_currentStateFacade.IsStarted)
        {
            return;
        }
        
        await AskNextWord(chatId);
    }

    private async Task AskNextWord(long chatId)
        {
            var (key, answers) = _vocabularyFacade.GetRandomQuestion();

            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: answers[0], callbackData: answers[0]),
                    InlineKeyboardButton.WithCallbackData(text: answers[1], callbackData: answers[1]),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: answers[2], callbackData: answers[2]),
                    InlineKeyboardButton.WithCallbackData(text: answers[3], callbackData: answers[3]),
                },
            });

            await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: $"{key}",
                replyMarkup: inlineKeyboard);
        }
    }
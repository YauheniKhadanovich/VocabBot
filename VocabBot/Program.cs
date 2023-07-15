using VocabBot.Feature.Bot.Services;
using VocabBot.Feature.Bot.Services.Impl;

IBotService botService = new BotService();
botService.Initialize();
Console.ReadLine();
using Microsoft.Extensions.DependencyInjection;
using VocabBot.Feature.Bot.Services;
using VocabBot.Feature.Bot.Services.Impl;
using VocabBot.Modules.CurrentState.Facade;
using VocabBot.Modules.CurrentState.Facade.Impl;
using VocabBot.Modules.CurrentState.Model;
using VocabBot.Modules.CurrentState.Model.Impl;
using VocabBot.Modules.Vocabulary.Facade;
using VocabBot.Modules.Vocabulary.Facade.Impl;
using VocabBot.Modules.Vocabulary.Model;
using VocabBot.Modules.Vocabulary.Model.Impl;

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddScoped<IBotService, BotService>()
    .AddScoped<ICurrentStateModel, CurrentStateModel>()
    .AddScoped<IVocabularyFacade, VocabularyFacade>()
    .AddScoped<IVocabularyModel, VocabularyModel>()
    .AddScoped<ICurrentStateFacade, CurrentStateFacade>()
    .BuildServiceProvider();

var botService = serviceProvider.GetService<IBotService>();
botService!.Initialize();

Console.ReadLine();
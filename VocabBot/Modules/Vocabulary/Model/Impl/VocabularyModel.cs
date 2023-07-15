namespace VocabBot.Modules.Vocabulary.Model.Impl;

public class VocabularyModel : IVocabularyModel
{ 
    private readonly string _vocabularyFilePath = @"/Users/yauheni/Work/English/Vocabulary";

    public Dictionary<string, List<string>> Dictionary { get; } = new();

    public bool TryInitialize()
    {
        if (!File.Exists(_vocabularyFilePath))
        {
            Console.WriteLine("Can't find file");
            return false;
        }

        try
        {
            var lines = File.ReadLines(_vocabularyFilePath);
            foreach (var line in lines)
            {
                ProcessLine(line);
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public KeyValuePair<string, List<string>> GetRandomQuestion()
    {
        var randomPair = Dictionary.ToArray()[new Random().Next(0, Dictionary.Count)];
        var word = randomPair.Key;
        var trueAnswers = randomPair.Value;
        var trueAnswer = randomPair.Value.ToArray()[new Random().Next(0, randomPair.Value.Count)];
        var wrongAnswers = Dictionary
            .Select(item => item.Value)
            .SelectMany(item => item)
            .Where(item => !trueAnswers.Contains(item))
            .Distinct();
        var options = wrongAnswers.OrderBy(_=>new Random().Next()).Take(3).ToList();
        options.Add(trueAnswer);
        return new KeyValuePair<string, List<string>>(word, options);
    }

    private void ProcessLine(string line)
    {
        var fixedLine = line.Trim().Replace("--", "#");
        var pair = fixedLine.Split("#");
        if (pair.Length != 2)
        {
            // TODO console
            return;
        }

        var key = pair[0].Trim().ToLowerInvariant();
        var values = pair[1].Split(",").ToList().Select(item => item.Trim().ToLowerInvariant()).ToList();
        Dictionary.Add(key, values);
    }
}
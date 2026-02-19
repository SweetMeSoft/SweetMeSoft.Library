using SweetMeSoft.Base;
using SweetMeSoft.Base.Tools;

using System.Text.Json;
using System.Text.RegularExpressions;

namespace SweetMeSoft.Tools;

public class Utils
{
    public static string GetException(Exception e)
    {
        return e.Message.Contains("See the inner exception") ? GetException(e.InnerException) : e.Message;
    }

    public static string GetRandomNumber(int lenght)
    {
        var result = "";
        var random = new Random();
        for (var i = 0; i < lenght; i++)
        {
            result += random.Next(0, 9);
        }

        return result;
    }

    public static void WriteToAPath(StreamFile stream, string path)
    {
        stream.Stream.Position = 0;
        var fileStream = new FileStream(path + "/" + stream.FileName, FileMode.Create, FileAccess.Write);
        stream.Stream.CopyTo(fileStream);
        fileStream.Dispose();
    }

    public static void WriteToAPath(List<StreamFile> streams, string path)
    {
        foreach (var stream in streams)
        {
            WriteToAPath(stream, path);
        }
    }

    public static List<StringMatch> StringMatchCompare(List<string> list, string chain, decimal threshold)
    {
        var result = new List<StringMatch>();

        foreach (var product in list)
        {
            var pairs1 = WordLetterPairs(chain.ToUpper());
            var pairs2 = WordLetterPairs(product.ToUpper());

            var intersections = 0m;

            for (var i = 0; i < pairs1.Count; i++)
            {
                for (var j = 0; j < pairs2.Count; j++)
                {
                    if (pairs1[i] == pairs2[j])
                    {
                        intersections++;
                        pairs2.RemoveAt(j);//Must remove the match to prevent "GGGG" from appearing to match "GG" with 100% success

                        break;
                    }
                }
            }

            var match = intersections / pairs1.Count;
            if (match > threshold)
            {
                result.Add(new StringMatch(product, match));
            }
        }

        return result;
    }

    /// <summary>
    /// Gets all letter pairs for each
    /// individual word in the string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static List<string> WordLetterPairs(string str)
    {
        List<string> AllPairs = new List<string>();

        // Tokenize the string and put the tokens/words into an array
        var Words = Regex.Split(str, @"\s");

        // For each word
        for (var w = 0; w < Words.Length; w++)
        {
            if (!string.IsNullOrEmpty(Words[w]))
            {
                // Find the pairs of characters
                var PairsInWord = LetterPairs(Words[w]);

                for (var p = 0; p < PairsInWord.Length; p++)
                {
                    AllPairs.Add(PairsInWord[p]);
                }
            }
        }

        return AllPairs;
    }

    /// <summary>
    /// Generates an array containing every
    /// two consecutive letters in the input string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string[] LetterPairs(string str)
    {
        var numPairs = str.Length - 1;

        var pairs = new string[numPairs];

        for (var i = 0; i < numPairs; i++)
        {
            pairs[i] = str.Substring(i, 2);
        }

        return pairs;
    }

    public static string MinifyJson(string json)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement, options);
        }
        catch (Exception)
        {
            return json;
        }
    }
}
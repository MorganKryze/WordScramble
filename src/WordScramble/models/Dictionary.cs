namespace WordScramble.models;
/// <summary>The dictionary creation class.</summary>
public class CustomDictionary
{
    #region Fields
    /// <summary>The language of the dictionary.</summary>
    public static string? s_Language;
    /// <summary>The static dictionary.</summary>
    public static Dictionary<int, string[]> s_Dict = new();
    /// <summary>The dynamic dictionary.</summary>
    public Dictionary<int, List<string>> ListDict = new();
    #endregion

    #region Constructor
    /// <summary>The constructor of the class.</summary>
    public CustomDictionary()
    {
        ListDict = s_Dict.ToDictionary(x => x.Key, x => x.Value.ToList());
    }
    #endregion
    #region Methods
    /// <summary>This method is used to create a static dictionary.</summary>
    public static void CreateDictionary()
    {
        s_Language ??= "FR";
        string[] lines = s_Language == "FR" ? File.ReadAllLines("dataDictionary/MotsPossiblesFR.txt") : File.ReadAllLines("dataDictionary/MotsPossiblesEN.txt");
        int[] Key = lines.Where((x, i) => i % 2 == 0).Select(x => int.Parse(x)).ToArray();
        string[][] Value = lines.Where((x, i) => i % 2 != 0).Select(x => x.Split(' ')).ToArray();
        s_Dict = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
    }
    #endregion
}
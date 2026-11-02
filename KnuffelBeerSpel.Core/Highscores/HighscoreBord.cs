namespace KnuffelBeerSpel.Core.Highscores;

/// <summary>
/// De top 10. Sorteert, knipt af en laat de opslag het bewaren.
/// Weet niet of dat een bestand of een database is.
/// </summary>
public class HighscoreBord
{
    public const int MaxAantal = 10;
    public const int MaxNaamLengte = 20;

    private readonly IHighscoreOpslag opslag;
    private List<Highscore> scores;

    public HighscoreBord(IHighscoreOpslag opslag)
    {
        this.opslag = opslag;
        scores = Sorteer(opslag.Laad()).ToList();
    }

    public IReadOnlyList<Highscore> Scores => scores;

    /// <summary>Haalt deze score de top 10?</summary>
    public bool IsHighscore(int score)
    {
        if (score <= 0) return false;
        return scores.Count < MaxAantal || score > scores[^1].Score;
    }

    /// <summary>Voegt een score toe en bewaart direct.</summary>
    public void VoegToe(string naam, int score, DateTime datum)
    {
        naam = (naam ?? "").Trim();
        if (naam.Length == 0) naam = "Anoniem";
        if (naam.Length > MaxNaamLengte) naam = naam[..MaxNaamLengte];

        scores.Add(new Highscore(naam, score, datum));
        scores = Sorteer(scores).ToList();
        opslag.Bewaar(scores);
    }

    // Hoogste score eerst; bij gelijke score wie het eerst haalde.
    private static IEnumerable<Highscore> Sorteer(IEnumerable<Highscore> lijst) =>
        lijst.OrderByDescending(s => s.Score)
             .ThenBy(s => s.Datum)
             .Take(MaxAantal);
}

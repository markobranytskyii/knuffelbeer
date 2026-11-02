using System.Text.Json;

namespace KnuffelBeerSpel.Core.Highscores;

/// <summary>Bewaart highscores in een JSON-bestand.</summary>
public class JsonHighscoreOpslag : IHighscoreOpslag
{
    private static readonly JsonSerializerOptions Opties = new() { WriteIndented = true };

    private readonly string bestandsPad;

    public JsonHighscoreOpslag(string bestandsPad)
    {
        if (string.IsNullOrWhiteSpace(bestandsPad))
            throw new ArgumentException("Pad mag niet leeg zijn.", nameof(bestandsPad));

        this.bestandsPad = bestandsPad;
    }

    public IReadOnlyList<Highscore> Laad()
    {
        if (!File.Exists(bestandsPad)) return Array.Empty<Highscore>();

        try
        {
            string json = File.ReadAllText(bestandsPad);
            return JsonSerializer.Deserialize<List<Highscore>>(json) ?? new List<Highscore>();
        }
        catch (JsonException)
        {
            // Kapot bestand: liever een leeg scorebord dan een spel dat crasht.
            return Array.Empty<Highscore>();
        }
    }

    public void Bewaar(IEnumerable<Highscore> scores)
    {
        string? map = Path.GetDirectoryName(bestandsPad);
        if (!string.IsNullOrEmpty(map)) Directory.CreateDirectory(map);

        string json = JsonSerializer.Serialize(scores.ToList(), Opties);
        File.WriteAllText(bestandsPad, json);
    }
}

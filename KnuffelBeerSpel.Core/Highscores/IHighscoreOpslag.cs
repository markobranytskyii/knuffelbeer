namespace KnuffelBeerSpel.Core.Highscores;

/// <summary>
/// Waar de highscores bewaard worden. Nu is dat een JSON-bestand, maar door deze
/// interface kan dat later een database (bijvoorbeeld SQLite) worden zonder dat
/// HighscoreBord verandert. In tests gebruik je een nep-opslag in het geheugen.
/// </summary>
public interface IHighscoreOpslag
{
    IReadOnlyList<Highscore> Laad();

    void Bewaar(IEnumerable<Highscore> scores);
}

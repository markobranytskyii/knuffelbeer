using KnuffelBeerSpel.Core.Highscores;

namespace KnuffelBeerSpel.Tests;

public class HighscoreTests
{
    private static readonly DateTime Vandaag = new(2026, 11, 2);

    [Fact]
    public void VoegToe_SorteertHoogsteScoreBovenaan()
    {
        var bord = new HighscoreBord(new GeheugenOpslag());

        bord.VoegToe("Anna", 50, Vandaag);
        bord.VoegToe("Bram", 300, Vandaag);
        bord.VoegToe("Cem", 120, Vandaag);

        Assert.Equal(new[] { "Bram", "Cem", "Anna" }, bord.Scores.Select(s => s.Naam));
    }

    [Fact]
    public void VoegToe_BewaartMaximaal10Scores()
    {
        var bord = new HighscoreBord(new GeheugenOpslag());

        for (int i = 1; i <= 15; i++) bord.VoegToe($"Speler{i}", i * 10, Vandaag);

        Assert.Equal(HighscoreBord.MaxAantal, bord.Scores.Count);
        Assert.Equal(150, bord.Scores[0].Score);
        Assert.Equal(60, bord.Scores[^1].Score);
    }

    [Fact]
    public void VoegToe_SlaatOpInDeOpslag()
    {
        var opslag = new GeheugenOpslag();
        var bord = new HighscoreBord(opslag);

        bord.VoegToe("Anna", 50, Vandaag);

        Assert.Single(opslag.Opgeslagen);
    }

    [Fact]
    public void VoegToe_LegeNaamWordtAnoniem()
    {
        var bord = new HighscoreBord(new GeheugenOpslag());

        bord.VoegToe("   ", 50, Vandaag);

        Assert.Equal("Anoniem", bord.Scores[0].Naam);
    }

    [Fact]
    public void VoegToe_TeLangeNaamWordtIngekort()
    {
        var bord = new HighscoreBord(new GeheugenOpslag());

        bord.VoegToe(new string('x', 50), 50, Vandaag);

        Assert.Equal(HighscoreBord.MaxNaamLengte, bord.Scores[0].Naam.Length);
    }

    [Fact]
    public void IsHighscore_BijVolBordAlleenAlsHogerDanLaatste()
    {
        var bord = new HighscoreBord(new GeheugenOpslag());
        for (int i = 1; i <= 10; i++) bord.VoegToe($"Speler{i}", i * 10, Vandaag);

        Assert.False(bord.IsHighscore(10));
        Assert.True(bord.IsHighscore(11));
    }

    [Fact]
    public void IsHighscore_NulPuntenTeltNiet()
    {
        var bord = new HighscoreBord(new GeheugenOpslag());

        Assert.False(bord.IsHighscore(0));
    }

    [Fact]
    public void JsonOpslag_BewaarEnLaad_GeeftDezelfdeScoresTerug()
    {
        string pad = Path.Combine(Path.GetTempPath(), $"knuffelbeer-test-{Guid.NewGuid()}.json");
        try
        {
            var opslag = new JsonHighscoreOpslag(pad);
            var scores = new[] { new Highscore("Anna", 50, Vandaag), new Highscore("Bram", 300, Vandaag) };

            opslag.Bewaar(scores);
            var geladen = new JsonHighscoreOpslag(pad).Laad();

            Assert.Equal<Highscore>(scores, geladen);
        }
        finally
        {
            File.Delete(pad);
        }
    }

    [Fact]
    public void JsonOpslag_BestandBestaatNiet_GeeftLegeLijst()
    {
        string pad = Path.Combine(Path.GetTempPath(), $"bestaat-niet-{Guid.NewGuid()}.json");

        Assert.Empty(new JsonHighscoreOpslag(pad).Laad());
    }

    [Fact]
    public void JsonOpslag_KapotBestand_GeeftLegeLijstInPlaatsVanCrash()
    {
        string pad = Path.Combine(Path.GetTempPath(), $"kapot-{Guid.NewGuid()}.json");
        try
        {
            File.WriteAllText(pad, "{ dit is geen geldige json");

            Assert.Empty(new JsonHighscoreOpslag(pad).Laad());
        }
        finally
        {
            File.Delete(pad);
        }
    }

    /// <summary>Nep-opslag in het geheugen, zodat tests geen bestanden nodig hebben.</summary>
    private class GeheugenOpslag : IHighscoreOpslag
    {
        public List<Highscore> Opgeslagen { get; } = new();

        public IReadOnlyList<Highscore> Laad() => Opgeslagen.ToList();

        public void Bewaar(IEnumerable<Highscore> scores)
        {
            Opgeslagen.Clear();
            Opgeslagen.AddRange(scores);
        }
    }
}

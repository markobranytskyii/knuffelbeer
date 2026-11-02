using KnuffelBeerSpel.Core;
using KnuffelBeerSpel.Core.Beren;

namespace KnuffelBeerSpel.Tests;

public class SpelTests
{
    // Vaste seed: elke testrun dezelfde "willekeur".
    private static Spel NieuwSpel() => new Spel(800, 600, new Random(42));

    [Fact]
    public void NieuwSpel_BegintMetDrieLevensEnNulPunten()
    {
        var spel = NieuwSpel();

        Assert.Equal(3, spel.Levens);
        Assert.Equal(0, spel.Score);
        Assert.Equal(1, spel.Level);
        Assert.False(spel.IsGameOver);
    }

    [Fact]
    public void KlikOpGewoneBeer_Geeft10Punten()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new GewoneBeer(100, 100, 1, 0));

        spel.Klik(100, 100);

        Assert.Equal(10, spel.Score);
    }

    [Fact]
    public void KlikOpSnelleBeer_Geeft25Punten()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new SnelleBeer(100, 100, 1, 0));

        spel.Klik(100, 100);

        Assert.Equal(25, spel.Score);
    }

    [Fact]
    public void KlikOpBozeBeer_KostEenLeven()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new BozeBeer(100, 100, 1, 0));

        spel.Klik(100, 100);

        Assert.Equal(2, spel.Levens);
        Assert.Equal(0, spel.Score);
    }

    [Fact]
    public void KlikOpGoudenBeer_GeeftPuntenEnExtraTijd()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new GoudenBeer(100, 100, 1, 0));

        spel.Klik(100, 100);

        Assert.Equal(100, spel.Score);
        Assert.Equal(Spel.SpeelTijd + GoudenBeer.BonusSeconden, spel.ResterendeTijd, 3);
    }

    [Fact]
    public void Klik_GeraakteBeerVerdwijnt()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new GewoneBeer(100, 100, 1, 0));

        spel.Klik(100, 100);

        Assert.Empty(spel.Beren);
    }

    [Fact]
    public void KlikNaast_VerandertNiets()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new GewoneBeer(100, 100, 1, 0));

        Beer? geraakt = spel.Klik(500, 500);

        Assert.Null(geraakt);
        Assert.Equal(0, spel.Score);
        Assert.Single(spel.Beren);
    }

    [Fact]
    public void KlikOpTweeBerenOverElkaar_AlleenDeBovensteTelt()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new GewoneBeer(100, 100, 1, 0));
        spel.VoegBeerToe(new BozeBeer(100, 100, 1, 0)); // later toegevoegd = bovenop

        spel.Klik(100, 100);

        Assert.Equal(2, spel.Levens);
        Assert.Single(spel.Beren);
        Assert.IsType<GewoneBeer>(spel.Beren[0]);
    }

    [Fact]
    public void DrieBozeBeren_IsGameOver()
    {
        var spel = NieuwSpel();
        for (int i = 0; i < 3; i++)
        {
            spel.VoegBeerToe(new BozeBeer(100, 100, 1, 0));
            spel.Klik(100, 100);
        }

        Assert.Equal(0, spel.Levens);
        Assert.True(spel.IsGameOver);
    }

    [Fact]
    public void NaZestigSeconden_IsGameOver()
    {
        var spel = NieuwSpel();

        spel.Update(Spel.SpeelTijd);

        Assert.Equal(0, spel.ResterendeTijd);
        Assert.True(spel.IsGameOver);
    }

    [Fact]
    public void NaGameOver_TeltEenKlikNietMeer()
    {
        var spel = NieuwSpel();
        spel.VoegBeerToe(new GewoneBeer(100, 100, 1, 0));
        spel.Update(Spel.SpeelTijd);

        spel.Klik(100, 100);

        Assert.Equal(0, spel.Score);
    }

    [Fact]
    public void Update_LaatBerenVerschijnen()
    {
        var spel = NieuwSpel();

        spel.Update(3);

        Assert.NotEmpty(spel.Beren);
    }

    [Fact]
    public void Update_NooitMeerDanMaximumBeren()
    {
        var spel = NieuwSpel();

        // Kleine stapjes, zoals de echte timer.
        for (int i = 0; i < 500; i++) spel.Update(0.1);

        Assert.True(spel.Beren.Count <= Spel.MaxBerenTegelijk);
    }

    [Fact]
    public void Level_GaatOmhoogPer200Punten()
    {
        var spel = NieuwSpel();

        spel.VoegPuntenToe(199);
        Assert.Equal(1, spel.Level);

        spel.VoegPuntenToe(1);
        Assert.Equal(2, spel.Level);
    }

    [Fact]
    public void HogerLevel_BerenKomenVakerEnLopenSneller()
    {
        var spel = NieuwSpel();
        double tussenpozeLevel1 = spel.SecondenTussenBeren;
        float snelheidLevel1 = spel.SnelheidsFactor;

        spel.VoegPuntenToe(Spel.PuntenPerLevel * 2);

        Assert.True(spel.SecondenTussenBeren < tussenpozeLevel1);
        Assert.True(spel.SnelheidsFactor > snelheidLevel1);
    }

    [Fact]
    public void VerliesLeven_GaatNietOnderNul()
    {
        var spel = NieuwSpel();

        for (int i = 0; i < 10; i++) spel.VerliesLeven();

        Assert.Equal(0, spel.Levens);
    }
}

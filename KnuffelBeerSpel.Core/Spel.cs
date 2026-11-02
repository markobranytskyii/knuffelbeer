using KnuffelBeerSpel.Core.Beren;

namespace KnuffelBeerSpel.Core;

/// <summary>
/// De spelregels: score, levens, level, tijd en welke beren er rondlopen.
/// Deze klasse weet niets van Windows Forms. De UI roept alleen Update() en Klik()
/// aan en leest de eigenschappen uit om te tekenen.
/// </summary>
public class Spel
{
    public const int StartLevens = 3;
    public const double SpeelTijd = 60;
    public const int PuntenPerLevel = 200;
    public const int MaxBerenTegelijk = 12;

    private readonly List<Beer> beren = new();
    private readonly BeerFabriek fabriek;
    private double tijdTotVolgendeBeer;

    /// <summary>Spel met echte willekeur, voor het echte spel.</summary>
    public Spel(float breedte, float hoogte)
        : this(breedte, hoogte, new Random())
    {
    }

    /// <summary>
    /// Spel met een zelf gekozen Random. In tests geef je bijvoorbeeld new Random(42) mee,
    /// zodat er elke keer precies dezelfde beren komen.
    /// </summary>
    public Spel(float breedte, float hoogte, Random random)
    {
        if (breedte <= 0) throw new ArgumentOutOfRangeException(nameof(breedte));
        if (hoogte <= 0) throw new ArgumentOutOfRangeException(nameof(hoogte));

        Breedte = breedte;
        Hoogte = hoogte;
        fabriek = new BeerFabriek(random);
        Levens = StartLevens;
        ResterendeTijd = SpeelTijd;
    }

    public float Breedte { get; private set; }
    public float Hoogte { get; private set; }
    public int Score { get; private set; }
    public int Levens { get; private set; }
    public double ResterendeTijd { get; private set; }

    /// <summary>Elke 200 punten een level hoger: level 1 bij 0 punten, level 2 bij 200, enz.</summary>
    public int Level => 1 + Score / PuntenPerLevel;

    public bool IsGameOver => Levens <= 0 || ResterendeTijd <= 0;

    /// <summary>Alleen-lezen, zodat de UI de lijst niet zelf kan aanpassen.</summary>
    public IReadOnlyList<Beer> Beren => beren;

    /// <summary>Hoe vaak er een nieuwe beer bijkomt. Wordt korter per level.</summary>
    public double SecondenTussenBeren => Math.Max(0.35, 1.2 - (Level - 1) * 0.15);

    /// <summary>Hoeveel sneller de beren lopen dan normaal. 20% extra per level.</summary>
    public float SnelheidsFactor => 1f + (Level - 1) * 0.2f;

    /// <summary>
    /// Laat de tijd verstrijken. De UI roept dit aan vanuit zijn timer;
    /// een test kan het direct aanroepen, bijvoorbeeld Update(60).
    /// </summary>
    public void Update(double seconden)
    {
        if (IsGameOver || seconden <= 0) return;

        ResterendeTijd = Math.Max(0, ResterendeTijd - seconden);

        foreach (Beer beer in beren)
        {
            beer.Update(seconden, Breedte, Hoogte);
        }
        beren.RemoveAll(b => b.IsWeg);

        tijdTotVolgendeBeer -= seconden;
        while (tijdTotVolgendeBeer <= 0)
        {
            if (beren.Count < MaxBerenTegelijk)
            {
                beren.Add(fabriek.MaakWillekeurigeBeer(Breedte, Hoogte, SnelheidsFactor));
            }
            tijdTotVolgendeBeer += SecondenTussenBeren;
        }
    }

    /// <summary>
    /// De speler klikt op (x, y). Alleen de bovenste beer op die plek telt.
    /// Spel vraagt niet welke soort beer het is: de beer bepaalt zelf wat er gebeurt.
    /// </summary>
    /// <returns>De geraakte beer, of null bij mis.</returns>
    public Beer? Klik(float x, float y)
    {
        if (IsGameOver) return null;

        // Van achter naar voor: de laatst getekende beer ligt bovenop.
        for (int i = beren.Count - 1; i >= 0; i--)
        {
            Beer beer = beren[i];
            if (beer.Raakt(x, y))
            {
                beer.WordtGeklikt(this);
                beren.RemoveAt(i);
                return beer;
            }
        }
        return null;
    }

    /// <summary>Voegt een specifieke beer toe. Handig in tests.</summary>
    public void VoegBeerToe(Beer beer)
    {
        ArgumentNullException.ThrowIfNull(beer);
        beren.Add(beer);
    }

    public void VoegPuntenToe(int punten)
    {
        if (punten < 0) throw new ArgumentOutOfRangeException(nameof(punten));
        Score += punten;
    }

    public void VerliesLeven()
    {
        if (Levens > 0) Levens--;
    }

    public void VoegTijdToe(double seconden)
    {
        if (seconden < 0) throw new ArgumentOutOfRangeException(nameof(seconden));
        ResterendeTijd += seconden;
    }

    /// <summary>Als het venster groter of kleiner wordt.</summary>
    public void PasGrootteAan(float breedte, float hoogte)
    {
        if (breedte > 0) Breedte = breedte;
        if (hoogte > 0) Hoogte = hoogte;
    }
}

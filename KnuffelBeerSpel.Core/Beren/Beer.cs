using System.Drawing;

namespace KnuffelBeerSpel.Core.Beren;

/// <summary>
/// Basisklasse voor alle beren. Abstract, want "een beer" zonder soort bestaat niet
/// in het spel: je maakt altijd een GewoneBeer, SnelleBeer, BozeBeer of GoudenBeer.
/// Wat alle beren gemeen hebben (bewegen, raken, basis-tekening) staat hier één keer.
/// </summary>
public abstract class Beer
{
    public const float StandaardStraal = 30f;

    private double resterendeTijd;

    protected Beer(float x, float y, float richtingX, float richtingY, float snelheid, double levensduur)
        : this(x, y, richtingX, richtingY, snelheid, levensduur, StandaardStraal)
    {
    }

    protected Beer(float x, float y, float richtingX, float richtingY, float snelheid, double levensduur, float straal)
    {
        if (snelheid < 0) throw new ArgumentOutOfRangeException(nameof(snelheid));
        if (straal <= 0) throw new ArgumentOutOfRangeException(nameof(straal));

        X = x;
        Y = y;
        Straal = straal;
        resterendeTijd = levensduur;

        // De richting wordt genormaliseerd (lengte 1), zodat "snelheid" altijd
        // pixels per seconde betekent, ongeacht welke richting is meegegeven.
        float lengte = MathF.Sqrt(richtingX * richtingX + richtingY * richtingY);
        if (lengte == 0)
        {
            richtingX = 1;
            lengte = 1;
        }
        SnelheidX = richtingX / lengte * snelheid;
        SnelheidY = richtingY / lengte * snelheid;
    }

    public float X { get; private set; }
    public float Y { get; private set; }
    public float Straal { get; }
    public float SnelheidX { get; private set; }
    public float SnelheidY { get; private set; }

    /// <summary>True als de beer uit het spel moet: geklikt of zijn tijd is op.</summary>
    public bool IsWeg { get; private set; }

    /// <summary>Hoeveel punten deze soort beer waard is. Elke subklasse bepaalt dit zelf.</summary>
    public abstract int Punten { get; }

    /// <summary>De vachtkleur. Elke subklasse bepaalt dit zelf.</summary>
    protected abstract Color Vachtkleur { get; }

    /// <summary>
    /// Beweegt de beer en laat hem tegen de randen van het speelveld stuiteren.
    /// </summary>
    public void Update(double seconden, float breedte, float hoogte)
    {
        if (IsWeg) return;

        X += SnelheidX * (float)seconden;
        Y += SnelheidY * (float)seconden;

        if (X - Straal < 0) { X = Straal; SnelheidX = Math.Abs(SnelheidX); }
        if (X + Straal > breedte) { X = breedte - Straal; SnelheidX = -Math.Abs(SnelheidX); }
        if (Y - Straal < 0) { Y = Straal; SnelheidY = Math.Abs(SnelheidY); }
        if (Y + Straal > hoogte) { Y = hoogte - Straal; SnelheidY = -Math.Abs(SnelheidY); }

        resterendeTijd -= seconden;
        if (resterendeTijd <= 0) IsWeg = true;
    }

    /// <summary>Ligt het punt (bijvoorbeeld een muisklik) binnen deze beer?</summary>
    public bool Raakt(float x, float y)
    {
        float dx = x - X;
        float dy = y - Y;
        return dx * dx + dy * dy <= Straal * Straal;
    }

    /// <summary>
    /// Wat er gebeurt als de speler op deze beer klikt. Standaard: punten erbij.
    /// Subklassen kunnen dit overschrijven (polymorfisme): BozeBeer kost een leven,
    /// GoudenBeer geeft extra tijd. Spel hoeft niet te weten welke soort het is.
    /// </summary>
    public virtual void WordtGeklikt(Spel spel)
    {
        spel.VoegPuntenToe(Punten);
        Verdwijn();
    }

    protected void Verdwijn() => IsWeg = true;

    /// <summary>
    /// Tekent de basis-beer: oren, hoofd, snuit, ogen en neus.
    /// Subklassen roepen base.Teken() aan en tekenen daarna hun eigen details.
    /// </summary>
    public virtual void Teken(ITekenaar tekenaar)
    {
        float r = Straal;
        Color snuitKleur = Color.FromArgb(245, 222, 179);

        // Oren
        tekenaar.TekenCirkel(X - r * 0.7f, Y - r * 0.7f, r * 0.35f, Vachtkleur);
        tekenaar.TekenCirkel(X + r * 0.7f, Y - r * 0.7f, r * 0.35f, Vachtkleur);
        tekenaar.TekenCirkel(X - r * 0.7f, Y - r * 0.7f, r * 0.18f, snuitKleur);
        tekenaar.TekenCirkel(X + r * 0.7f, Y - r * 0.7f, r * 0.18f, snuitKleur);

        // Hoofd en snuit
        tekenaar.TekenCirkel(X, Y, r, Vachtkleur);
        tekenaar.TekenCirkel(X, Y + r * 0.35f, r * 0.4f, snuitKleur);

        // Ogen en neus
        tekenaar.TekenCirkel(X - r * 0.35f, Y - r * 0.2f, r * 0.12f, Color.Black);
        tekenaar.TekenCirkel(X + r * 0.35f, Y - r * 0.2f, r * 0.12f, Color.Black);
        tekenaar.TekenCirkel(X, Y + r * 0.22f, r * 0.12f, Color.Black);
    }
}

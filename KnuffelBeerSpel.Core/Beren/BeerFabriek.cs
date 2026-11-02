namespace KnuffelBeerSpel.Core.Beren;

/// <summary>
/// Maakt willekeurige beren. Dit is de enige plek die alle soorten kent:
/// een nieuwe soort beer toevoegen = een nieuwe subklasse + één regel hier.
/// Spel zelf hoeft daarvoor niet te veranderen.
/// </summary>
public class BeerFabriek
{
    // Kansen in procenten. Samen 100.
    public const int KansGouden = 5;
    public const int KansBoos = 20;
    public const int KansSnel = 25;

    private readonly Random random;

    public BeerFabriek(Random random)
    {
        this.random = random;
    }

    public Beer MaakWillekeurigeBeer(float breedte, float hoogte, float snelheidsFactor)
    {
        // Ergens binnen het veld, niet half buiten de rand.
        float marge = Beer.StandaardStraal;
        float x = marge + (float)random.NextDouble() * Math.Max(0, breedte - 2 * marge);
        float y = marge + (float)random.NextDouble() * Math.Max(0, hoogte - 2 * marge);

        double hoek = random.NextDouble() * 2 * Math.PI;
        float richtingX = (float)Math.Cos(hoek);
        float richtingY = (float)Math.Sin(hoek);

        int worp = random.Next(100);

        if (worp < KansGouden)
            return new GoudenBeer(x, y, richtingX, richtingY, snelheidsFactor);
        if (worp < KansGouden + KansBoos)
            return new BozeBeer(x, y, richtingX, richtingY, snelheidsFactor);
        if (worp < KansGouden + KansBoos + KansSnel)
            return new SnelleBeer(x, y, richtingX, richtingY, snelheidsFactor);

        return new GewoneBeer(x, y, richtingX, richtingY, snelheidsFactor);
    }
}

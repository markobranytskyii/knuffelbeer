using System.Drawing;

namespace KnuffelBeerSpel.Core.Beren;

/// <summary>Kleiner en sneller dan een gewone beer, en daarom meer punten waard.</summary>
public class SnelleBeer : Beer
{
    public const float BasisSnelheid = 260f;
    public const double Levensduur = 5;

    public SnelleBeer(float x, float y, float richtingX, float richtingY, float snelheidsFactor = 1f)
        : base(x, y, richtingX, richtingY, BasisSnelheid * snelheidsFactor, Levensduur, StandaardStraal * 0.8f)
    {
    }

    public override int Punten => 25;

    protected override Color Vachtkleur => Color.Peru;

    public override void Teken(ITekenaar tekenaar)
    {
        // Snelheidsstreepjes achter de beer, tegen de bewegingsrichting in.
        float lengte = MathF.Sqrt(SnelheidX * SnelheidX + SnelheidY * SnelheidY);
        if (lengte > 0)
        {
            float terugX = -SnelheidX / lengte;
            float terugY = -SnelheidY / lengte;
            for (int i = -1; i <= 1; i++)
            {
                // Loodrecht op de richting een beetje opschuiven per streepje.
                float zijX = -terugY * i * Straal * 0.5f;
                float zijY = terugX * i * Straal * 0.5f;
                float startX = X + terugX * Straal * 1.1f + zijX;
                float startY = Y + terugY * Straal * 1.1f + zijY;
                tekenaar.TekenLijn(startX, startY, startX + terugX * Straal, startY + terugY * Straal, Color.Gray, 2f);
            }
        }

        base.Teken(tekenaar);
    }
}

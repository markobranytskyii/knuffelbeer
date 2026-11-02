using System.Drawing;

namespace KnuffelBeerSpel.Core.Beren;

/// <summary>De standaard beer: 10 punten, gemiddelde snelheid.</summary>
public class GewoneBeer : Beer
{
    public const float BasisSnelheid = 120f;
    public const double Levensduur = 6;

    public GewoneBeer(float x, float y, float richtingX, float richtingY, float snelheidsFactor = 1f)
        : base(x, y, richtingX, richtingY, BasisSnelheid * snelheidsFactor, Levensduur)
    {
    }

    public override int Punten => 10;

    protected override Color Vachtkleur => Color.SaddleBrown;
}

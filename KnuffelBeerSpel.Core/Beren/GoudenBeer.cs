using System.Drawing;

namespace KnuffelBeerSpel.Core.Beren;

/// <summary>Zeldzaam en snel weer weg. Geeft veel punten én extra speeltijd.</summary>
public class GoudenBeer : Beer
{
    public const float BasisSnelheid = 180f;
    public const double Levensduur = 2.5;
    public const double BonusSeconden = 5;

    public GoudenBeer(float x, float y, float richtingX, float richtingY, float snelheidsFactor = 1f)
        : base(x, y, richtingX, richtingY, BasisSnelheid * snelheidsFactor, Levensduur)
    {
    }

    public override int Punten => 100;

    protected override Color Vachtkleur => Color.Gold;

    public override void WordtGeklikt(Spel spel)
    {
        base.WordtGeklikt(spel);          // de gewone punten...
        spel.VoegTijdToe(BonusSeconden);  // ...plus de bonus
    }

    public override void Teken(ITekenaar tekenaar)
    {
        base.Teken(tekenaar);

        // Een klein glinstertje rechtsboven.
        float sx = X + Straal * 0.9f;
        float sy = Y - Straal * 0.9f;
        float s = Straal * 0.3f;
        tekenaar.TekenLijn(sx - s, sy, sx + s, sy, Color.White, 2f);
        tekenaar.TekenLijn(sx, sy - s, sx, sy + s, Color.White, 2f);
    }
}

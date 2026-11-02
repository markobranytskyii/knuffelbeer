using System.Drawing;

namespace KnuffelBeerSpel.Core.Beren;

/// <summary>Niet op klikken! Een boze beer kost een leven in plaats van punten op te leveren.</summary>
public class BozeBeer : Beer
{
    public const float BasisSnelheid = 100f;
    public const double Levensduur = 7;

    public BozeBeer(float x, float y, float richtingX, float richtingY, float snelheidsFactor = 1f)
        : base(x, y, richtingX, richtingY, BasisSnelheid * snelheidsFactor, Levensduur)
    {
    }

    public override int Punten => 0;

    protected override Color Vachtkleur => Color.DarkRed;

    public override void WordtGeklikt(Spel spel)
    {
        // Geen base.WordtGeklikt(): deze beer geeft geen punten.
        spel.VerliesLeven();
        Verdwijn();
    }

    public override void Teken(ITekenaar tekenaar)
    {
        base.Teken(tekenaar);

        // Boze wenkbrauwen schuin boven de ogen.
        float r = Straal;
        tekenaar.TekenLijn(X - r * 0.55f, Y - r * 0.5f, X - r * 0.15f, Y - r * 0.3f, Color.Black, 3f);
        tekenaar.TekenLijn(X + r * 0.55f, Y - r * 0.5f, X + r * 0.15f, Y - r * 0.3f, Color.Black, 3f);
    }
}

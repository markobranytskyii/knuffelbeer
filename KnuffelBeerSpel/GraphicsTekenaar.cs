using System.Drawing.Drawing2D;
using KnuffelBeerSpel.Core;

namespace KnuffelBeerSpel;

/// <summary>
/// De Windows Forms-versie van ITekenaar: vertaalt "teken een cirkel"
/// naar echte GDI+-aanroepen op een Graphics-object.
/// </summary>
internal sealed class GraphicsTekenaar : ITekenaar
{
    private readonly Graphics graphics;

    public GraphicsTekenaar(Graphics graphics)
    {
        this.graphics = graphics;
    }

    public void TekenCirkel(float middenX, float middenY, float straal, Color kleur)
    {
        using var kwast = new SolidBrush(kleur);
        graphics.FillEllipse(kwast, middenX - straal, middenY - straal, straal * 2, straal * 2);
    }

    public void TekenLijn(float x1, float y1, float x2, float y2, Color kleur, float dikte)
    {
        using var pen = new Pen(kleur, dikte) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        graphics.DrawLine(pen, x1, y1, x2, y2);
    }
}

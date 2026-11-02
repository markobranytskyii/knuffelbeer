using System.Drawing;

namespace KnuffelBeerSpel.Core;

/// <summary>
/// Alles wat een beer nodig heeft om zichzelf te tekenen.
/// De beren weten niet of ze op een Windows Forms-scherm worden getekend
/// of ergens anders: de UI levert een implementatie van deze interface.
/// Zo blijft de Core los van Windows Forms.
/// </summary>
public interface ITekenaar
{
    void TekenCirkel(float middenX, float middenY, float straal, Color kleur);

    void TekenLijn(float x1, float y1, float x2, float y2, Color kleur, float dikte);
}

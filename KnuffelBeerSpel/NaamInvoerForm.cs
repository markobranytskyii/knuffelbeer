using KnuffelBeerSpel.Core.Highscores;

namespace KnuffelBeerSpel;

/// <summary>Klein venster om je naam in te vullen bij een nieuwe highscore.</summary>
internal sealed class NaamInvoerForm : Form
{
    private readonly TextBox naamVeld;

    public NaamInvoerForm(int score)
    {
        Text = "Nieuwe highscore!";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(320, 120);

        var uitleg = new Label
        {
            Text = $"Je hebt {score} punten! Wat is je naam?",
            AutoSize = true,
            Location = new Point(12, 12),
        };

        naamVeld = new TextBox
        {
            Location = new Point(12, 40),
            Width = 296,
            MaxLength = HighscoreBord.MaxNaamLengte,
        };

        var opslaanKnop = new Button
        {
            Text = "Opslaan",
            DialogResult = DialogResult.OK,
            Location = new Point(152, 80),
            Width = 75,
        };

        var overslaanKnop = new Button
        {
            Text = "Overslaan",
            DialogResult = DialogResult.Cancel,
            Location = new Point(233, 80),
            Width = 75,
        };

        Controls.AddRange(new Control[] { uitleg, naamVeld, opslaanKnop, overslaanKnop });
        AcceptButton = opslaanKnop;
        CancelButton = overslaanKnop;
    }

    public string Naam => naamVeld.Text;
}

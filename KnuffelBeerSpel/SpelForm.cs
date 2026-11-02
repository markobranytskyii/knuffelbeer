using System.Diagnostics;
using System.Drawing.Drawing2D;
using KnuffelBeerSpel.Core;
using KnuffelBeerSpel.Core.Beren;
using KnuffelBeerSpel.Core.Highscores;

namespace KnuffelBeerSpel;

/// <summary>
/// Het spelvenster. Deze klasse doet alleen drie dingen:
/// 1. de tijd doorgeven aan Spel (timer),
/// 2. muisklikken doorgeven aan Spel,
/// 3. tekenen wat Spel zegt.
/// Alle spelregels staan in KnuffelBeerSpel.Core.
/// </summary>
public partial class SpelForm : Form
{
    private static readonly Color Achtergrond = Color.FromArgb(198, 228, 190);
    private static readonly Font HudLettertype = new("Segoe UI", 14, FontStyle.Bold);
    private static readonly Font TitelLettertype = new("Segoe UI", 32, FontStyle.Bold);
    private static readonly Font TekstLettertype = new("Segoe UI", 12);

    private readonly System.Windows.Forms.Timer spelTimer = new() { Interval = 16 }; // ~60 beelden per seconde
    private readonly Stopwatch stopwatch = new();
    private readonly HighscoreBord highscores;
    private readonly List<ZwevendeTekst> zwevendeTeksten = new();

    private Spel spel;
    private bool isGestart;
    private bool gameOverAfgehandeld;

    public SpelForm()
    {
        InitializeComponent();
        DoubleBuffered = true; // voorkomt flikkeren

        string pad = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "KnuffelBeerSpel",
            "highscores.json");
        highscores = new HighscoreBord(new JsonHighscoreOpslag(pad));

        spel = new Spel(ClientSize.Width, ClientSize.Height);
        spelTimer.Tick += SpelTimer_Tick;
    }

    private void StartNieuwSpel()
    {
        spel = new Spel(ClientSize.Width, ClientSize.Height);
        zwevendeTeksten.Clear();
        isGestart = true;
        gameOverAfgehandeld = false;
        stopwatch.Restart();
        spelTimer.Start();
    }

    private void SpelTimer_Tick(object? sender, EventArgs e)
    {
        // Echte verstreken tijd meten, zodat het spel op elke computer even snel gaat.
        // Maximaal 0,1 s per stap, zodat een hapering geen enorme sprong geeft.
        double seconden = Math.Min(stopwatch.Elapsed.TotalSeconds, 0.1);
        stopwatch.Restart();

        spel.Update(seconden);

        foreach (ZwevendeTekst tekst in zwevendeTeksten) tekst.Update(seconden);
        zwevendeTeksten.RemoveAll(t => t.IsKlaar);

        Invalidate();

        if (spel.IsGameOver && !gameOverAfgehandeld)
        {
            gameOverAfgehandeld = true;
            spelTimer.Stop();
            HandelGameOverAf();
        }
    }

    private void HandelGameOverAf()
    {
        Invalidate();
        if (!highscores.IsHighscore(spel.Score)) return;

        using var dialoog = new NaamInvoerForm(spel.Score);
        if (dialoog.ShowDialog(this) == DialogResult.OK)
        {
            highscores.VoegToe(dialoog.Naam, spel.Score, DateTime.Now);
        }
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (!isGestart || spel.IsGameOver || e.Button != MouseButtons.Left) return;

        int scoreVoor = spel.Score;
        int levensVoor = spel.Levens;
        double tijdVoor = spel.ResterendeTijd;

        if (spel.Klik(e.X, e.Y) is null) return;

        // De UI kijkt alleen wát er veranderd is, niet wélke soort beer het was.
        // Zo hoeft deze code niet aangepast te worden voor een nieuwe soort beer.
        int punten = spel.Score - scoreVoor;
        if (punten > 0) zwevendeTeksten.Add(new ZwevendeTekst($"+{punten}", e.X, e.Y, Color.DarkGreen));
        if (spel.Levens < levensVoor) zwevendeTeksten.Add(new ZwevendeTekst("-1 leven", e.X, e.Y, Color.DarkRed));
        if (spel.ResterendeTijd > tijdVoor) zwevendeTeksten.Add(new ZwevendeTekst("+tijd!", e.X, e.Y + 24, Color.DarkGoldenrod));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        bool magStarten = !isGestart || (spel.IsGameOver && gameOverAfgehandeld);
        if (e.KeyCode == Keys.Space && magStarten) StartNieuwSpel();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        // OnResize kan al tijdens InitializeComponent() worden aangeroepen, vóór spel bestaat.
        spel?.PasGrootteAan(ClientSize.Width, ClientSize.Height);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Achtergrond);

        if (!isGestart)
        {
            TekenStartScherm(g);
            return;
        }

        var tekenaar = new GraphicsTekenaar(g);
        foreach (Beer beer in spel.Beren)
        {
            beer.Teken(tekenaar); // polymorfisme: elke soort tekent zichzelf
        }

        foreach (ZwevendeTekst tekst in zwevendeTeksten) tekst.Teken(g);

        TekenHud(g);

        if (spel.IsGameOver) TekenGameOverScherm(g);
    }

    private void TekenHud(Graphics g)
    {
        string links = $"Score: {spel.Score}    Level: {spel.Level}";
        string rechts = $"Levens: {new string('♥', spel.Levens)}    Tijd: {Math.Ceiling(spel.ResterendeTijd)}";

        g.DrawString(links, HudLettertype, Brushes.Black, 12, 10);
        SizeF maat = g.MeasureString(rechts, HudLettertype);
        g.DrawString(rechts, HudLettertype, Brushes.Black, ClientSize.Width - maat.Width - 12, 10);
    }

    private void TekenStartScherm(Graphics g)
    {
        TekenGecentreerd(g, "Knuffelbeer Vangen", TitelLettertype, Brushes.SaddleBrown, 60);
        TekenGecentreerd(g, "Klik op de beren voor punten. Je hebt 60 seconden en 3 levens.", TekstLettertype, Brushes.Black, 130);

        // Dezelfde Teken()-methode als in het spel: de uitleg gebruikt de echte beren.
        var voorbeelden = new (Beer Beer, string Uitleg)[]
        {
            (new GewoneBeer(0, 0, 1, 0), "Gewone beer: 10 punten"),
            (new SnelleBeer(0, 0, 1, 0), "Snelle beer: 25 punten"),
            (new GoudenBeer(0, 0, 1, 0), "Gouden beer: 100 punten + 5 sec"),
            (new BozeBeer(0, 0, 1, 0), "Boze beer: NIET klikken! (-1 leven)"),
        };

        var tekenaar = new GraphicsTekenaar(g);
        float startY = 200;
        float x = ClientSize.Width / 2f - 170;
        for (int i = 0; i < voorbeelden.Length; i++)
        {
            float y = startY + i * 75;
            // De voorbeeldbeer staat op (0, 0); we schuiven het tekenvlak naar de juiste plek.
            GraphicsState staat = g.Save();
            g.TranslateTransform(x, y);
            voorbeelden[i].Beer.Teken(tekenaar);
            g.Restore(staat);
            g.DrawString(voorbeelden[i].Uitleg, TekstLettertype, Brushes.Black, x + 50, y - 10);
        }

        TekenGecentreerd(g, "Druk op SPATIE om te beginnen", HudLettertype, Brushes.DarkGreen, startY + voorbeelden.Length * 75 + 10);
    }

    private void TekenGameOverScherm(Graphics g)
    {
        using var waas = new SolidBrush(Color.FromArgb(190, Color.White));
        g.FillRectangle(waas, ClientRectangle);

        string reden = spel.Levens <= 0 ? "Je levens zijn op!" : "De tijd is om!";
        TekenGecentreerd(g, "Game over", TitelLettertype, Brushes.DarkRed, 40);
        TekenGecentreerd(g, $"{reden}  Je score: {spel.Score} (level {spel.Level})", HudLettertype, Brushes.Black, 105);

        TekenGecentreerd(g, "Highscores", HudLettertype, Brushes.SaddleBrown, 160);
        float y = 195;
        if (highscores.Scores.Count == 0)
        {
            TekenGecentreerd(g, "Nog geen highscores", TekstLettertype, Brushes.Gray, y);
        }
        for (int i = 0; i < highscores.Scores.Count; i++)
        {
            Highscore h = highscores.Scores[i];
            TekenGecentreerd(g, $"{i + 1}. {h.Naam}  —  {h.Score}", TekstLettertype, Brushes.Black, y + i * 26);
        }

        if (gameOverAfgehandeld)
        {
            TekenGecentreerd(g, "Druk op SPATIE om opnieuw te spelen", HudLettertype, Brushes.DarkGreen, ClientSize.Height - 60);
        }
    }

    private void TekenGecentreerd(Graphics g, string tekst, Font lettertype, Brush kleur, float y)
    {
        SizeF maat = g.MeasureString(tekst, lettertype);
        g.DrawString(tekst, lettertype, kleur, (ClientSize.Width - maat.Width) / 2, y);
    }


    /// <summary>"+10" of "-1 leven" dat even omhoog zweeft na een klik. Puur iets van de UI.</summary>
    private sealed class ZwevendeTekst
    {
        private const double Duur = 0.8;
        private static readonly Font Lettertype = new("Segoe UI", 14, FontStyle.Bold);

        private readonly string tekst;
        private readonly float x;
        private readonly Color kleur;
        private float y;
        private double leeftijd;

        public ZwevendeTekst(string tekst, float x, float y, Color kleur)
        {
            this.tekst = tekst;
            this.x = x;
            this.y = y;
            this.kleur = kleur;
        }

        public bool IsKlaar => leeftijd >= Duur;

        public void Update(double seconden)
        {
            leeftijd += seconden;
            y -= (float)(40 * seconden);
        }

        public void Teken(Graphics g)
        {
            int doorzichtigheid = (int)(255 * Math.Clamp(1 - leeftijd / Duur, 0, 1));
            using var kwast = new SolidBrush(Color.FromArgb(doorzichtigheid, kleur));
            g.DrawString(tekst, Lettertype, kwast, x, y);
        }
    }
}

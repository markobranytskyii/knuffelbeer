using KnuffelBeerSpel.Core;
using KnuffelBeerSpel.Core.Beren;

namespace KnuffelBeerSpel.Tests;

public class BeerTests
{
    [Fact]
    public void Raakt_KlikInHetMidden_IsRaak()
    {
        var beer = new GewoneBeer(100, 100, 1, 0);

        Assert.True(beer.Raakt(100, 100));
    }

    [Fact]
    public void Raakt_KlikNaastDeBeer_IsMis()
    {
        var beer = new GewoneBeer(100, 100, 1, 0);

        Assert.False(beer.Raakt(100 + Beer.StandaardStraal + 1, 100));
    }

    [Fact]
    public void Update_BeweegtMetSnelheidMaalTijd()
    {
        var beer = new GewoneBeer(100, 100, 1, 0);

        beer.Update(0.5, 1000, 1000);

        Assert.Equal(100 + GewoneBeer.BasisSnelheid * 0.5f, beer.X, 3);
        Assert.Equal(100, beer.Y, 3);
    }

    [Fact]
    public void Update_TegenRechterrand_StuitertTerug()
    {
        var beer = new GewoneBeer(950, 100, 1, 0);

        beer.Update(1, 1000, 1000);

        Assert.True(beer.SnelheidX < 0);
        Assert.True(beer.X + beer.Straal <= 1000);
    }

    [Fact]
    public void Update_NaLevensduur_IsWeg()
    {
        var beer = new GewoneBeer(500, 500, 1, 0);

        beer.Update(GewoneBeer.Levensduur + 0.1, 1000, 1000);

        Assert.True(beer.IsWeg);
    }

    [Fact]
    public void SnelleBeer_IsSnellerDanGewoneBeer()
    {
        var gewoon = new GewoneBeer(0, 0, 1, 0);
        var snel = new SnelleBeer(0, 0, 1, 0);

        Assert.True(snel.SnelheidX > gewoon.SnelheidX);
    }

    [Fact]
    public void SnelheidsFactor_MaaktBeerSneller()
    {
        var normaal = new GewoneBeer(0, 0, 1, 0, 1f);
        var level3 = new GewoneBeer(0, 0, 1, 0, 1.4f);

        Assert.Equal(normaal.SnelheidX * 1.4f, level3.SnelheidX, 3);
    }

    [Fact]
    public void Punten_VerschillenPerSoort()
    {
        Assert.Equal(10, new GewoneBeer(0, 0, 1, 0).Punten);
        Assert.Equal(25, new SnelleBeer(0, 0, 1, 0).Punten);
        Assert.Equal(0, new BozeBeer(0, 0, 1, 0).Punten);
        Assert.Equal(100, new GoudenBeer(0, 0, 1, 0).Punten);
    }

    [Fact]
    public void Teken_RoeptDeTekenaarAan()
    {
        var tekenaar = new TellendeTekenaar();
        var beer = new BozeBeer(100, 100, 1, 0);

        beer.Teken(tekenaar);

        Assert.True(tekenaar.Cirkels > 0);
        Assert.True(tekenaar.Lijnen > 0); // de boze wenkbrauwen
    }

    [Fact]
    public void BeerFabriek_MetZelfdeSeed_GeeftZelfdeBeren()
    {
        var a = new BeerFabriek(new Random(42));
        var b = new BeerFabriek(new Random(42));

        for (int i = 0; i < 20; i++)
        {
            Beer beerA = a.MaakWillekeurigeBeer(800, 600, 1f);
            Beer beerB = b.MaakWillekeurigeBeer(800, 600, 1f);
            Assert.Equal(beerA.GetType(), beerB.GetType());
            Assert.Equal(beerA.X, beerB.X);
        }
    }

    [Fact]
    public void BeerFabriek_MaaktAlleSoortenBeren()
    {
        var fabriek = new BeerFabriek(new Random(1));
        var soorten = new HashSet<Type>();

        for (int i = 0; i < 1000; i++)
        {
            soorten.Add(fabriek.MaakWillekeurigeBeer(800, 600, 1f).GetType());
        }

        Assert.Contains(typeof(GewoneBeer), soorten);
        Assert.Contains(typeof(SnelleBeer), soorten);
        Assert.Contains(typeof(BozeBeer), soorten);
        Assert.Contains(typeof(GoudenBeer), soorten);
    }

    private class TellendeTekenaar : ITekenaar
    {
        public int Cirkels { get; private set; }
        public int Lijnen { get; private set; }

        public void TekenCirkel(float middenX, float middenY, float straal, System.Drawing.Color kleur) => Cirkels++;

        public void TekenLijn(float x1, float y1, float x2, float y2, System.Drawing.Color kleur, float dikte) => Lijnen++;
    }
}

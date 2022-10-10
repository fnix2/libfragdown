using NUnit.Framework;
using libfragdown;

namespace LibfragdownTests;

public class LibfragdownTests
{
    private static readonly object[] ArgumentsForTestingSmartCoordinate =
    {
    new object[] { new ImageCoordinates(), new ImageCoordinates(5, 5), new ImageCoordinates(4, 0), new int[] { 1, 1, 2 }, true },
    new object[] { new ImageCoordinates(), new ImageCoordinates(5, 5), new ImageCoordinates(0, 5), new int[] { 10, 10, 10, 10 }, false },
    new object[] { new ImageCoordinates(1, 2), new ImageCoordinates(7, 10), new ImageCoordinates(4, 4), new int[] { 3, 6, 3, 7 }, true },
    new object[] { new ImageCoordinates(), new ImageCoordinates(2, 3), new ImageCoordinates(2, 3), Enumerable.Repeat(1, 13), false },
    };

    [TestCaseSource(nameof(ArgumentsForTestingSmartCoordinate))]
    public void SmartCoordinateCheck(ImageCoordinates initCoordinates, ImageCoordinates maxCoordinates, ImageCoordinates expectedCoordinates, IEnumerable<int> numbersToAdd, bool isSuccessExpected)
    {
        SmartCoordinate smartCoordinate = new(maxCoordinates, initCoordinates);
        bool isSuccessReal = true;
        foreach (int addCoor in numbersToAdd)
        {
            (smartCoordinate, isSuccessReal) = smartCoordinate + addCoor;
        }

        Assert.Multiple(() =>
        {
            Assert.That(smartCoordinate.Current, Is.EqualTo(expectedCoordinates), "SmartCoordinate: real Current does not correspond expected Current!");
            Assert.That(isSuccessReal, Is.EqualTo(isSuccessExpected), "SmartCoordinate: real add operation status does not correspond expected operation status!");
        });
    }

    [TestCase(10, 30)]
    [TestCase(100, 100)]
    [TestCase(1000, 5)]
    [TestCase(5, 1)]
    [TestCase(1, 1)]
    [TestCase(0, 0)]
    public void FindMaxCoordinatesTest(int horExpected, int verExpected)
    {
        ImageCoordinates maxCoordinatesExpected = new(horExpected, verExpected);
        RemoteCoordinateStateTest remoteState = new(maxCoordinatesExpected);
        ImageCoordinates maxCoordinatesReal = new MaxCoordinatesSearch(remoteState).FindMaxCoordinates();
        Assert.That(maxCoordinatesReal, Is.EqualTo(maxCoordinatesExpected));
    }

    [TestCase(1, 1)]
    [TestCase(0, 0)]
    [TestCase(1, 50)]
    [TestCase(50, 1)]
    [TestCase(0, 50)]
    [TestCase(50, 0)]
    [TestCase(50, 50)]
    public void CoordinatesGeneratorOneTest(int hor, int ver)
    {
        ImageCoordinates maxCor = new(hor, ver);
        CoordinatesGenerator corGen = new(maxCor);
        int expectedCount = (ver + 1) * (hor + 1);
        Assert.That(corGen, Has.Count.EqualTo(expectedCount));
        int realCount = 0;
        foreach (ImageCoordinates cor in corGen)
        {
            realCount++;
        }
        Assert.That(realCount, Is.EqualTo(expectedCount));
    }

}

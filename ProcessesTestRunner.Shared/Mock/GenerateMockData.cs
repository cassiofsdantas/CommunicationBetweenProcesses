using AutoFixture;

namespace ProcessesTestRunner.Shared.Mock;

public static class GenerateMockData
{
    public static IEnumerable<T> GenerateTimes<T>(int times = 10000)
    {
        Fixture fixture = new();

        return fixture.CreateMany<T>(times);
    }
}
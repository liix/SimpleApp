namespace MyWebApi.Tests;

public class UnitTest1
{
    [Fact]
    public void AppHasProgramcs()
    {
        var type = typeof(Program);
        Assert.NotNull(type);
    }
}

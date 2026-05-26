public class BCryptTests
{
    [Fact]
    public void Verify_BonMdp_RetourneTrue()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("monMdp", workFactor: 8);
        Assert.True(BCrypt.Net.BCrypt.Verify("monMdp", hash));
    }

    [Fact]
    public void Verify_MauvaisMdp_RetourneFalse()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("bonMdp", workFactor: 8);
        Assert.False(BCrypt.Net.BCrypt.Verify("mauvaisMdp", hash));
    }
}

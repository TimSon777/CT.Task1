namespace FaceDetector.Services;

public interface IIamTokenService
{
    Task<string> GetTokenAsync();
}

public sealed class IamTokenService : IIamTokenService
{
    public Task<string> GetTokenAsync()
    {
        var token = "t1.9euelZrJiYvLl42akM-WkMbGkZSWxu3rnpWax8jLjY7Oz8-Lxs7PipzLjcvl8_coLA5V-e8dN0w3_N3z92haC1X57x03TDf8zef1656VmpSMipOMzpCVjJKRloyenJWP7_zF656VmpSMipOMzpCVjJKRloyenJWP.poPsI9YLKGiJmks6dcK9C8JKlmb_lRymsoktkYgQtuDLFkstC7MCFVtkwH8bT5IeQD09vqAdkw2v2ywS4N0-Aw";
        return Task.FromResult(token);
    }
}

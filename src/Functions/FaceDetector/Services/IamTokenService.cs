namespace FaceDetector.Services;

public interface IIamTokenService
{
    Task<string> GetTokenAsync();
}

public sealed class IamTokenService : IIamTokenService
{
    public Task<string> GetTokenAsync()
    {
        var token = "t1.9euelZrMz52Sy5WJl5fOyI2Lj5STyO3rnpWax8jLjY7Oz8-Lxs7PipzLjcvl8_cHAnlU-e8JWUYo_N3z90cwdlT57wlZRij8zef1656VmorMmZ6RlMbLls-ayc7MkcmJ7_zF656VmorMmZ6RlMbLls-ayc7MkcmJ.uVkI5Z2_6lH_28vHscDsS702j0iCM1mBxi_wEvqJX0IQvZ26A-Z0AC_YH7zxSaOz3LHuiWzPWgH6v_R8nJUGAw";
        return Task.FromResult(token);
    }
}

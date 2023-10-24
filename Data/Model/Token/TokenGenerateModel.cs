using System.ComponentModel.DataAnnotations;

namespace main.Data.Model.Token;

public class TokenGenerateModel
{
    [EmailAddress]
    public string Email
    {
        get;
        set;
    } = "";

    [MinLength(6)]
    [MaxLength(18)]
    public string Password
    {
        get;
        set;
    } = "";
}
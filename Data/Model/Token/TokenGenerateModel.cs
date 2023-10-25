using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace main.Data.Model.Token;

public class TokenGenerateModel
{
    [JsonProperty("email")]
    [EmailAddress]
    public string Email
    {
        get;
        set;
    } = "";

    [JsonProperty("password")]
    [MinLength(6)]
    [MaxLength(18)]
    public string Password
    {
        get;
        set;
    } = "";
}
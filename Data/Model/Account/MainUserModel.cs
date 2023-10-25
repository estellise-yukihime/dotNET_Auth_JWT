using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace main.Data.Model.Account;

public class MainUserModel
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
    
    [JsonProperty("name")]
    [MinLength(2)]
    public string Name
    {
        get;
        set;
    } = "";

}
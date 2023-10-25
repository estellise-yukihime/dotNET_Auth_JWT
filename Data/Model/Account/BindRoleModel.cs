using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace main.Data.Model.Account;

public class BindRoleModel
{
    [JsonProperty("email")]
    [EmailAddress]
    public string Email
    {
        get;
        set;
    } = "";
    
    [JsonProperty("role")]
    [MinLength(1)]
    public string Role
    {
        get;
        set;
    } = "";
}
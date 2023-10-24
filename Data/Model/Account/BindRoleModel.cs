using System.ComponentModel.DataAnnotations;

namespace main.Data.Model.Account;

public class BindRoleModel
{
    [EmailAddress]
    public string Email
    {
        get;
        set;
    } = "";
    
    [MinLength(1)]
    public string Role
    {
        get;
        set;
    } = "";
}
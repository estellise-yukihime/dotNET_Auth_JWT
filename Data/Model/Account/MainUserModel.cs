using System.ComponentModel.DataAnnotations;

namespace main.Data.Model.Account;

public class MainUserModel
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
    
    [MinLength(2)]
    public string Name
    {
        get;
        set;
    } = "";

}
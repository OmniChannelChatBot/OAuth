using System;
using System.ComponentModel.DataAnnotations;

namespace OAuth.Model
{
    public class CheckUserNameModel
    {
        [Required]
        public string UserName { get; set; }
    }
}

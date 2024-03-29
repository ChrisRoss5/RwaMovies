﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RwaMovies.Models.Shared.Auth
{
    public class NewPasswordRequest
    {
        public AuthRequest AuthRequest { get; set; } = null!;
        [DisplayName("New password"), DataType(DataType.Password)]
        public string NewPassword1 { get; set; } = null!;
        [DisplayName("Confirm new password"), DataType(DataType.Password)]
        [Compare("NewPassword1", ErrorMessage = "The new password and confirmation password do not match.")]
        public string NewPassword2 { get; set; } = null!;

    }
}

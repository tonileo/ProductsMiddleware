﻿using System.ComponentModel.DataAnnotations;

namespace ProductsMiddleware.Models.Dto
{
    public class LoginRequestDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}

﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Modelos
{
    public class Role : IdentityRole
    {
        public string Descripcion { get; set; }
    }
}

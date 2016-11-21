﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueMoney.Common
{
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public static class ErrorMessages
    {
        public const string Required = "Это поле не может быть пустым";
        public const string Invalid = "Поле \"{0}\" содержит некорректное значение";
    }
}

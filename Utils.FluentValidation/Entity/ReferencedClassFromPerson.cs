using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.FluentValidation.Entity
{
    class ReferencedClassFromPerson
    {
        public string Property { get; set; }

        public override string ToString()
        {
            return $"Property value: {Property}";
        }
    }
}

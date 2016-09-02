using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Hotrod
{
    [Serializable]
    class Person
    {
        public string Name { get; set; }

        public string FirstName { get; set; }
        
        public int Age { get; set; }

        public override string ToString()
        {
            return $"Person [Name={Name + " " + FirstName}, Age={Age}]";
        }
    }
}

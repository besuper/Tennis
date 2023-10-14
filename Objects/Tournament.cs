using DemoClasses.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Tournament
    {
        private int id;
        private Edition edition;
        private List<Competition> competitions = new List<Competition>();
    }
}

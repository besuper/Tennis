using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClasses.Objects
{
    internal class Court
    {
        private int id;
        private string name;

        private List<Match> matches = new List<Match>();

        public Court(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public void AddMatch(Match match)
        {
            matches.Add(match);
        }

        public bool IsAvailable(DateTime date)
        {
            bool available = true;

            foreach (Match match in matches)
            {
                DateTime minDate = match.Date.AddMinutes(-10);
                DateTime maxDate = match.Date.AddMinutes(10);

                if (date > minDate && date < maxDate)
                {
                    available = false;
                    break;
                }
            }

            return available;
        }
    }
}

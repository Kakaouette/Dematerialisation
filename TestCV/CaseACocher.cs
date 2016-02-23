using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    public class CaseACocher 
    {
        public Point coord { get; set; }
        public string nom { get; set; }

        public CaseACocher(Point p, string n)
        {
            this.coord = p;
            this.nom = n;
        }
    }
}

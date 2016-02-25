using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerisation_GIST
{
    public class ZoneInfo
    {
        public Rectangle coord { get; set; }
        public string nom { get; set; }

        /*
        public ZoneInfo(Rectangle r, string n)
        {
            this.coord = r;
            this.nom = n;
        }
        */

        public ZoneInfo(int x, int y, int width, int height, string n)
        {
            this.coord = new Rectangle(x,y,width,height);
            this.nom = n;
        }
    }
}

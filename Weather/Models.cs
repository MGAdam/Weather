using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    public class Content
    {
        public Cord cordinates { get; set; }
        public Weather weather { get; set; }
        public string baseone { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
    }
}

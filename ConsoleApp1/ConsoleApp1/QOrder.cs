using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AFNToAFD
{
    public class QOrder
    {
        private string q;
        public int Order { get; set; }
        public int OrderForSequence { get; set; }

        public string Q
        {
            get
            {
                return this.q;
            }
            set
            {
                Order = Convert.ToInt32(Regex.Replace(value, "[^0-9]", ""));
                this.q = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimerToysTwo.Model
{
    public class AdjustParms
    {
        public string TimerKey { get; set; }
        public int SecondsOffset { get; set; }
    }

    public class RenameParms
    {
        public string TimerKey { get; set; }
        public string NewName { get; set; }
    }
}
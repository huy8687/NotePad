using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Note_
{
    class Key
    {
        public String Ques { get; set; }
        public String Ans { get; set; }
        public Key() { }
        public Key(String q, String a) {
            Ques = q;
            Ans = a;
        }
    }
}

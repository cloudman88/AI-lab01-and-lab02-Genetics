using System;
using System.CodeDom;

namespace Genetics
{
    class StringGen : Gen
    {
        private string _str;              // the string           
        public string Str
        {
            get { return _str; }
            set { _str = value; }
        }       
        public StringGen()
        {
            _str = "";
            _fitness = 0;
        }
        public StringGen(String str,uint fitness = 0)
        {
            _str = str;
            _fitness = fitness;
        }
    }
}

using System;
using System.CodeDom;

namespace Genetics
{
    class StringGen // old name was GaStruct in the given code
    {

        private string _str;              // the string
        private uint _fitness;           // its fitness
        public string Str
        {
            get { return _str; }
            set { _str = value; }
        }
        public uint Fitness
        {
            get { return _fitness; }
            set { _fitness = value; }
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

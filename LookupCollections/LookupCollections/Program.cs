using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;

namespace LookupCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            ListDictionary list = new ListDictionary(new CaseInsensitiveComparer(CultureInfo.InvariantCulture));

            list["Estados Unidos"] = "United States of America";
            list["Canadá"] = "Canada";
            list["España"] = "Spain";


            Console.WriteLine(list["españa"]);
            Console.WriteLine(list["CANADÁ"]);
            Console.Read();
        }
    }
}

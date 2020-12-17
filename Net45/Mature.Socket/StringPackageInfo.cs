using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public class StringPackageInfo: IkeyedPackageInfo<string>
    {
        public string Key { get; set; }
        public string Body { get; set; }
        public string[] Parameters { get; set; }
    }
}

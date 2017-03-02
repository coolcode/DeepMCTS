using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
     class Helper
    {
        public static void CopyBytes(byte[] destination, byte[] source)
        {
            source.CopyTo(destination, 0);
        }

    }
}

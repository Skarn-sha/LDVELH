using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDVELH.role.tools {
    class DataConverter {

        private static bool IsInt(string sVal) {
            foreach (char c in sVal) {
                int iN = (int)c;
                if ((iN > 57) || (iN < 48))
                    return false;
            }
            return true;
        }

        internal static int ToInt(string value) {
            int index = 0;
            if (value != null && value != "" && IsInt(value)) {
                index = int.Parse(value);
            }
            return index;
        }
    }
}

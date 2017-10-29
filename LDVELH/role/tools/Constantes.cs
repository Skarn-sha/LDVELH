using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDVELH.role.tools {
    public static class Constantes {
        /** File names **/
        public const string HERO_FILE_NAME = "hero.xml";
        public const string BOOK_FILE_NAME = "smdf.xml";

        /** Game phase **/
        public const int START_HERO_CREATION_PHASE = 0;
        public const int ENTITLEMENT_PHASE = 1;
        public const int STAMINA_PHASE = 2;
        public const int LUCK_PHASE = 3;
        public const int EXPLORATION_PHASE = 4;
        public const int FIGHT_PHASE = 5;

    }
}

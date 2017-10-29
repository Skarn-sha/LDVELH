using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDVELH.role.entity {
    class Enemy {
        private string name;
        private int entitlement;
        private int stamina;
        private int power;

        public string Name { get => name; set => name = value; }
        public int Entitlement { get => entitlement; set => entitlement = value; }
        public int Stamina { get => stamina; set => stamina = value; }
        public int Power { get => power; set => power = value; }

        public Enemy(string name, int entitlement, int stamina) {
            this.name = name;
            this.entitlement = entitlement;
            this.stamina = stamina;
        }

        public Boolean IsKO() {
            return stamina == 0;
        }
    }
}

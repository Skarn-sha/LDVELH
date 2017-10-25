using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDVELH.role.tools {
    class Dice {
        private static Random rand = new Random(); //note this new item
        // Fields
        private int _value;

        // Constructor
        public Dice() {
            _value = 1;
        }

        // Value property
        public int Value {
            get { return _value; }
        }

        // Rolls the die
        public void Roll() {
            _value = rand.Next(6) + 1; //no need to refer to the mainForm
        }

    }

}

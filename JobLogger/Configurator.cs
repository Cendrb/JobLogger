using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger
{
    class Configurator
    {
        private static Configurator instance;

        public static Configurator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configurator();
                }

                return instance;
            }
        }

        private Configurator()
        {
        }


    }
}

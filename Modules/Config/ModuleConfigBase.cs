using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Config
{
    public abstract class ModuleConfigBase
    {
        /// <summary>
        /// Wether the module should exist
        /// </summary>
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;
    }
}

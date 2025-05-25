using System.ComponentModel;

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
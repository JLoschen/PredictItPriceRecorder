using Menards.Merch.BOA.Core;

namespace PluginGUI.Models
{
    public class PluginConfig
    {
        public HostServerType HostType { get; set; }
        public HostServerType TeradataHostType { get; set; }
        public string TestEmail { get; set; }
        public int RestartDelay { get; set; }
        public string PluginName { get; set; }
    }
}

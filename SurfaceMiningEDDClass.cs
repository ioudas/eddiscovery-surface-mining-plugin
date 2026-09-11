using System;
using System.Runtime.InteropServices;
using BaseUtils.Icons;
using EDDDLLInterfaces;

namespace EDSurfaceMiningOverlay
{
    public class SurfaceMiningEDDClass
    {
        public static EDDDLLIF.EDDCallBacks GlobalCallbacks;

        public string EDDInitialise(string v, string dllfolder, EDDDLLIF.EDDCallBacks callbacks)
        {
            GlobalCallbacks = callbacks;
            System.Diagnostics.Trace.WriteLine("EDSurfaceMiningOverlay Initialised");
            
            System.Drawing.Image icon = null;
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("EDSurfaceMiningOverlay.SurfaceMiningOverlay.png"))
                {
                    if (stream != null)
                        icon = System.Drawing.Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("EDSurfaceMiningOverlay failed to load icon: " + ex.Message);
            }

            // Register the panel with EDDiscovery
            callbacks.AddPanel(
                "EDSurfaceMiningOverlay", 
                typeof(UserControlSurfaceMining), 
                "Surface Mining Map", 
                "SurfaceMiningMap", 
                "Surface Mining Map Overlay", 
                icon
            );
            
            // Return our version number
            return "1.0.0.0";
        }

        public void EDDTerminate()
        {
            System.Diagnostics.Trace.WriteLine("EDSurfaceMiningOverlay Terminated");
        }
    }
}

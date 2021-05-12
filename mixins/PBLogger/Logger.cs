using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public class Logger
    {
        private IMyTextSurface tty;
        private bool enableLogging;

        public Logger(IMyProgrammableBlock instance, bool enable)
        {
            if (instance != null && instance.SurfaceCount != 0)
            {
                // store and initialize the PB terminal for logging
                tty = instance.GetSurface(0);
                tty.ContentType = ContentType.TEXT_AND_IMAGE;
            }
            enableLogging = enable;
        }

        public void log(string text, bool append = true)
        {
            if (!enableLogging || tty == null)
            {
                return;
            }

            if (String.IsNullOrEmpty(text))
            {
                tty.WriteText("", append); // no newline for empty values
            }
            else
            {
                tty.WriteText($"{text}\n", append);
            }
        }

        public void clear () {
            log("", false);
        }
    }
}

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable; 
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class   Program : MyGridProgram
    {
        /*
         * This script will monitor airlocks (a pair of doors
         * designed to keep you from letting the air out) and will
         * disable the closed door when the other door is open.
         *
         * To group two doors together, name them like "Airlock 1a"
         * and "Airlock 1b". The next group will be "Airlock 2a" and
         * "[...]2b" and so on.
         */

        // Customize this to enable logging to the PB terminal screen
        private bool enableLogging = false;

        private IMyTextSurface tty;
        private Logger logger;

        public Program()
        {
            logger = new Logger(Me, enableLogging); 

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        { }

        public void Main(string argument, UpdateType updateSource)
        {
            // clear terminal
            logger.clear();

            List<IMyDoor> doors = new List<IMyDoor>();
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors);

            doors = doors.Where(d => d.CustomName.Contains("Airlock")).OrderBy(e => e.CustomName).ToList();

            var doorDict = new Dictionary<string, List<IMyDoor>>();

            for (var i = 1; i <= (doors.Count / 2); i++)
            { // divided by two because an airlock has 2 doors.  This should be improved.
                doorDict.Add($"Airlock {i}", doors.Where(d => d.CustomName.Contains($"Airlock {i}")).ToList());
            }

            foreach (KeyValuePair<string, List<IMyDoor>> entry in doorDict)
            {
                List<IMyDoor> doorGroup = entry.Value;
                List<IMyDoor> openDoors = doorGroup.Where(d => d.Status != DoorStatus.Closed).ToList();
                List<IMyDoor> closedDoors = doorGroup.Where(d => d.Status == DoorStatus.Closed).ToList();

                logger.log($"Group {entry.Key} has {openDoors.Count} open doors!");

                if (openDoors.Count > 0)
                {
                    // at least one door is open, so disable all non-open doors.
                    foreach (IMyDoor door in closedDoors)
                    {
                        door.Enabled = false;
                    }
                }
                else
                {
                    // all doors in this group are closed, so they can all be re-enabled.
                    foreach (IMyDoor door in doorGroup)
                    {
                        door.Enabled = true;
                    }
                }
            }
        }
    }
}

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
    partial class Program : MyGridProgram
    {
        /*
         * This script (when run) will either return the drilling rig rotors to their
         * normal rhythm or will return rotors to their starting positions,
         * depending on the current status (it should switch back and forth
         * each time it is run)
         */

        // Customize this to enable logging to the PB terminal screen
        private bool enableLogging = false;
        private Logger logger;

        public Program()
        {
            logger = new Logger(Me, enableLogging);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // clear console
            logger.clear();

            List<IMyMotorStator> rotors = GetRotors();

            foreach (var rotor in rotors.OrderBy(r => r.CustomName).ToList())
            {
                logger.log(rotor.CustomName.ToString());
                if (rotor.LowerLimitDeg != float.MinValue || rotor.UpperLimitDeg != float.MaxValue)
                {
                    resetLimits(rotor);
                }
                else
                {
                    updateLimits(rotor);
                }

                List<ITerminalAction> actions = new List<ITerminalAction>();
                ITerminalAction turnOn = rotor.GetActionWithName("OnOff_On");
                turnOn.Apply(rotor);
            }
        } //main

        void updateLimits(IMyMotorStator rotor)
        {
            logger.log($"back to starting positions! L={rotor.LowerLimitDeg}, U={rotor.UpperLimitDeg}");

            if (rotor.TargetVelocityRPM > 0) // clockwise
            {
                // if it's moving right, we want it to keep moving right.
                rotor.UpperLimitDeg = 360;
            }
            else
            {
                // if it's moving left, we want it to keep moving left.
                rotor.LowerLimitDeg = -90;
            }
        }

        void resetLimits(IMyMotorStator rotor) {
            logger.log($"back to normal! L={(int)rotor.LowerLimitDeg}, U={(int)rotor.UpperLimitDeg}");
            logger.log($"{rotor.LowerLimitDeg} > {rotor.UpperLimitDeg}");
            logger.log($"{float.MinValue}");

            rotor.LowerLimitDeg = float.MinValue;
            rotor.UpperLimitDeg = float.MaxValue;
            rotor.LowerLimitDeg = float.MinValue;
            rotor.UpperLimitDeg = float.MaxValue;
        }

        public List<IMyMotorStator> GetRotors()
        {
            List<IMyMotorStator> rotors = new List<IMyMotorStator>();
            GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors);

            logger.log(rotors.Count.ToString());

            return rotors;
        }
    }
}

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
        private List<IMyTerminalBlock> blocks;
        private List<KeyValuePair<IMyTerminalBlock, IMySlimBlock>> blocksAndSlims;
        private bool enableLogging = true;
        private Logger logger;
        public Program()
        {
            logger = new Logger(Me, enableLogging);
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            blocks = GetBlocksInCurrentGrid(Me, GridTerminalSystem);
            blocksAndSlims = blocks.Select(
                block => new KeyValuePair<IMyTerminalBlock, IMySlimBlock>(block, block.CubeGrid.GetCubeBlock(block.Position))
            ).ToList();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            logger.clear(); // TODO only clear when we have changes to draw

            foreach (IMyTerminalBlock block in blocks)
            {
                block.ShowOnHUD = false;
            }

            List<IMyTerminalBlock> damagedBlocks = GetTop10DamagedBlocks();

            if (damagedBlocks.Count == 0) {
                logger.log("No damaged terminal blocks detected.");
                return;
            }

            foreach (IMyTerminalBlock damagedBlock in damagedBlocks)
            {
                damagedBlock.ShowOnHUD = true;
            }

            string count = damagedBlocks.Count == 10 ? "10+" : damagedBlocks.Count.ToString();
            logger.log($"{count} damaged block(s) detected:");

            double percentage;

            logger.log(String.Join("\n", damagedBlocks.Select(block => {
                string roundedPercentage 
                    = Double.TryParse(block.CustomData, out percentage) 
                        ? Math.Round(percentage, 2).ToString() 
                        : "??";
                return $"{block.CustomName}: {roundedPercentage}%";
            })));
        }

        private List<IMyTerminalBlock> GetBlocksInCurrentGrid(IMyProgrammableBlock pbInstance, IMyGridTerminalSystem GTS)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GTS.GetBlocks(blocks);

            return blocks.Where(block => block.CubeGrid == pbInstance.CubeGrid).ToList();
        }

        private List<IMyTerminalBlock> GetTop10DamagedBlocks()
        {
            double percentage;
            return blocksAndSlims.Select(kvp => {
                float hull = kvp.Value.MaxIntegrity - kvp.Value.CurrentDamage;
                float perc = 100 * (hull / kvp.Value.MaxIntegrity);
                kvp.Key.CustomData = perc.ToString();

                return kvp.Key;
            })
            .Where(block => Double.TryParse(block.CustomData, out percentage) && percentage < 100)
            .OrderBy(block => Double.TryParse(block.CustomData, out percentage) ? percentage : 12.225)
            .Take(10)
            .ToList();
        }


    }
}

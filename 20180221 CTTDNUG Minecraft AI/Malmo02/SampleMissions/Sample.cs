using System;
using System.Threading;
using Microsoft.Research.Malmo;
using System.IO;
using System.Windows.Forms;
using MalmoCSharpLauncher.Shared;
using System.Collections.Generic;

namespace MalmoCSharpLauncher
{
    public class Sample
    {
        // This is a sample that moves in a random direction
        public static void Execute(AgentHost agentHost, ListBox lstMessage, Panel pnl3x3, Panel pnlDiagData, PictureBox pictureBoxMineCraft, CheckBox chkFullDebug)
        {
            WorldState worldState;
            Random rand = new Random();
            // main loop:
            do
            {
                // Get Worldstate
                worldState = agentHost.getWorldState();

                agentHost.sendCommand("move 1");
                agentHost.sendCommand(string.Format("turn {0}", rand.NextDouble()));
                Thread.Sleep(500);

                // Log Mission                
                Observation objObservation = RunMission.LogMission(worldState, lstMessage, pnl3x3, pnlDiagData, chkFullDebug);

                if (worldState.is_mission_running && worldState.video_frames.Count > 0)
                {
                    // Converts the Malmo ByteVector to a Bitmap and display in pictureBoxMineCraft
                    pictureBoxMineCraft.Image = ImageConvert.GetImageFromByteArray(worldState.video_frames[0].pixels);
                    pictureBoxMineCraft.Invalidate();
                    pictureBoxMineCraft.Refresh();
                }
            }
            while (worldState.is_mission_running);

            lstMessage.Items.Insert(0, "Mission has stopped.");
        }
    }

}

using System;
using System.Threading;
using Microsoft.Research.Malmo;
using System.IO;
using System.Windows.Forms;
using MalmoCSharpLauncher.Shared;
using System.Collections.Generic;

namespace MalmoCSharpLauncher
{
    public class Tutorial4
    {
        // This is a sample that moves in a random direction
        public static void Execute(AgentHost agentHost, ListBox lstMessage, Panel pnl3x3, Panel pnlDiagData, PictureBox pictureBoxMineCraft, CheckBox chkFullDebug)
        {
            WorldState worldState;
            agentHost.sendCommand("hotbar.9 1");
            agentHost.sendCommand("hotbar.9 0");
            agentHost.sendCommand("pitch 0.2");
            Thread.Sleep(1);
            agentHost.sendCommand("pitch 0");
            agentHost.sendCommand("move 1");
            agentHost.sendCommand("attack 1");

            // main loop:
            do
            {
                Thread.Sleep(10);

                // Get Worldstate
                worldState = agentHost.getWorldState();

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
            lstMessage.Refresh();
        }
    }

}

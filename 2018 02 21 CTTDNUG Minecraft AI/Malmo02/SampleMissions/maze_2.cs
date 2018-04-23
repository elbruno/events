using System;
using System.Threading;
using Microsoft.Research.Malmo;
using System.IO;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using MalmoCSharpLauncher.Shared;
using System.Collections.Generic;
using System.Drawing;

namespace MalmoCSharpLauncher
{
    public class maze2
    {
        // Get the tiles around the agent        
        public static void Execute(AgentHost agentHost, ListBox lstMessage, Panel pnl3x3, Panel pnlDiagData, PictureBox pictureBoxMineCraft, CheckBox chkFullDebug)
        {
            WorldState worldState;
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            // main loop:
            do
            {
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

                if (objObservation.LineOfSight != null)
                {
                    // See if there is room to move 
                    if ((Convert.ToDouble(objObservation.LineOfSight.z) - (objObservation.ZPos) > 1))
                    {
                        // Move
                        agentHost.sendCommand("move 1");
                        Thread.Sleep(500);
                    }
                    else
                    {
                        agentHost.sendCommand(string.Format("turn {0}", 1));
                        Thread.Sleep(500);
                    }
                }
            }
            while (worldState.is_mission_running);

            lstMessage.Items.Insert(0, "Mission has stopped.");
        }
    }
}

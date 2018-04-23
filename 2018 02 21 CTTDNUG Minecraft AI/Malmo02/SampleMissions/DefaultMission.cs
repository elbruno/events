using System;
using System.Threading;
using Microsoft.Research.Malmo;
using System.IO;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using MalmoCSharpLauncher.Shared;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Microsoft.Research.Malmo.AgentHost;

namespace MalmoCSharpLauncher
{
    public class DefaultMission
    {
        public static void Execute(AgentHost agentHost, ListBox lstMessage, Panel pnl3x3, Panel pnlDiagData, PictureBox pictureBoxMineCraft, CheckBox chkFullDebug)
        {
            WorldState worldState;
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();

            agentHost.setObservationsPolicy(ObservationsPolicy.LATEST_OBSERVATION_ONLY);
            agentHost.setVideoPolicy(VideoPolicy.LATEST_FRAME_ONLY);

            // main loop:
            do
            {
                // Get Worldstate
                worldState = agentHost.getWorldState();
                Thread.Sleep(500);

                // Make an Observation
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

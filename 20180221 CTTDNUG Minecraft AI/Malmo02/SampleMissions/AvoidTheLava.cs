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
    public class AvoidTheLava
    {
        public static void Execute(AgentHost agentHost, ListBox lstMessage, Panel pnl3x3, Panel pnlDiagData, PictureBox pictureBoxMineCraft, CheckBox chkFullDebug)
        {
            WorldState worldState;
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();

            agentHost.setObservationsPolicy(ObservationsPolicy.LATEST_OBSERVATION_ONLY);
            agentHost.setVideoPolicy(VideoPolicy.LATEST_FRAME_ONLY);

            string strMoveCommand = "";
            // main loop:
            do
            {
                // Get Worldstate
                worldState = agentHost.getWorldState();
                Thread.Sleep(500);

                // Make an Observation
                Observation objObservation = RunMission.LogMission(worldState, lstMessage, pnl3x3, pnlDiagData, chkFullDebug);

                // Only proceed if we are able to get back an Observation
                if (objObservation.floor3x3 != null)
                {
                    // Check for "lava" anywhere around us
                    if (!
                        (
                        (objObservation.floor3x3[1].ToString() == "lava") ||
                        (objObservation.floor3x3[4].ToString() == "lava") ||
                        (objObservation.floor3x3[7].ToString() == "lava")
                        ))
                    {
                        // There is no Lava -- keep moving forward
                        strMoveCommand = String.Format("{0} {1}", "move", 1);
                        agentHost.sendCommand(strMoveCommand);
                        lstMessage.Items.Insert(0, strMoveCommand);
                    }
                    else
                    {
                        // There is lava nearby
                        for (int i = 0; i < 8; i++)
                        {
                            if (objObservation.floor3x3[i].ToString() == "lava")
                            {
                                lstMessage.Items.Insert(0, String.Format("Lava found at block: {0}", i));
                            }
                        }

                        // Turn and move
                        strMoveCommand = String.Format("{0} {1}", "turn", 1);
                        agentHost.sendCommand(strMoveCommand);
                        lstMessage.Items.Insert(0, strMoveCommand);

                        strMoveCommand = String.Format("{0} {1}", "move", 1);
                        agentHost.sendCommand(strMoveCommand);
                        lstMessage.Items.Insert(0, strMoveCommand);
                    }
                }

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

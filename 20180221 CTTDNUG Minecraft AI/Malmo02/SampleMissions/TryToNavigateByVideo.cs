using System;
using System.Threading;
using Microsoft.Research.Malmo;
using System.IO;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using MalmoCSharpLauncher.Shared;
using System.Collections.Generic;
using System.Drawing;
using static Microsoft.Research.Malmo.AgentHost;

namespace MalmoCSharpLauncher
{
    public class TryToNavigateByVideo
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

                // Log Mission                
                Observation objObservation = RunMission.LogMission(worldState, lstMessage, pnl3x3, pnlDiagData, chkFullDebug);
                var yaw = objObservation.Yaw;

                string current_yaw_delta = "";
                if (worldState.is_mission_running && worldState.video_frames.Count > 0)
                {
                    // do we have new Video?
                    if (worldState.video_frames.Count > 0)
                    {                        
                        current_yaw_delta = processFrame(worldState.video_frames[0].pixels);
                        if (current_yaw_delta != "")
                        {
                            agentHost.sendCommand(String.Format("turn {0}", current_yaw_delta));
                        }
                    }
                    else
                    {
                        agentHost.sendCommand(String.Format("turn {0}", yaw));
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

        #region private static string processFrame(ByteVector pixels)
        private static string processFrame(ByteVector pixels)
        {
            // This was adapted from Python 
            // currently it does not appear to work

            string current_yaw_delta = "";
            var current_yaw_delta_from_depth = 0f;
            var video_width = 320;
            var video_height = 240;

            var y = Convert.ToInt32(video_height / 2);
            var rowstart = y * video_width;

            var v = 0;
            var v_max = 0;
            var v_max_pos = 0;
            var v_min = 0;
            var v_min_pos = 0;

            var dv = 0;
            var dv_max = 0;
            var dv_max_pos = 0;
            var dv_max_sign = false;

            var d2v = 0;
            var d2v_max = 0;
            var d2v_max_pos = 0;
            var d2v_max_sign = false;

            for (int x = 0; x < video_width; x++)
            {
                var nv = pixels[(rowstart + x) * 4 + 3];
                var ndv = nv - v;
                var nd2v = ndv - dv;

                if (nv > v_max || x == 0)
                {
                    v_max = nv;
                    v_max_pos = x;
                }

                if (nv < v_min || x == 0)
                {
                    v_min = nv;
                    v_min_pos = x;
                }

                if (Math.Abs(ndv) > dv_max || x == 1)
                {
                    dv_max = Math.Abs(ndv);
                    dv_max_pos = x;
                    dv_max_sign = (ndv > 0);
                }

                if (Math.Abs(nd2v) > d2v_max || x == 2)
                {
                    d2v_max = Math.Abs(nd2v);
                    d2v_max_pos = x;
                    d2v_max_sign = (nd2v > 0);
                }

                d2v = nd2v;
                dv = ndv;
                v = nv;
            }

            //logger.info("d2v, dv, v: " + str(d2v) + ", " + str(dv) + ", " + str(v))

            // We want to steer towards the greatest d2v (ie the biggest discontinuity in the gradient of the depth map).
            // If it's a possitive value, then it represents a rapid change from close to far - eg the left-hand edge of a gap.
            // Aiming to put this point in the leftmost quarter of the screen will cause us to aim for the gap.
            // If it's a negative value, it represents a rapid change from far to close - eg the right-hand edge of a gap.
            // Aiming to put this point in the rightmost quarter of the screen will cause us to aim for the gap.
            float edge;
            if (dv_max_sign)
            {
                edge = video_width / 4;
            }
            else
            {
                edge = 3 * video_width / 4;
            }

            // Now, if there is something noteworthy in d2v, steer according to the above comment:
            if (d2v_max > 8)
            {
                current_yaw_delta_from_depth = ((d2v_max_pos - edge) / video_width);
            }
            else
            {
                // Nothing obvious to aim for, so aim for the farthest point:
                if (v_max < 255)
                {
                    current_yaw_delta_from_depth = ((v_max_pos / video_width) - (0.5f));
                }
                else
                {
                    // No real data to be had in d2v or v, so just go by the direction we were already travelling in:
                    if (current_yaw_delta_from_depth < 0)
                    {
                        current_yaw_delta = "-1";
                    }
                    else
                    {
                        current_yaw_delta = "1";
                    }
                }
            }

            return current_yaw_delta;
        }
        #endregion

    }
}

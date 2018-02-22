using Microsoft.Research.Malmo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace MalmoCSharpLauncher.Shared
{
    class RunMission
    {
        public static Observation LogMission(WorldState worldState, ListBox lstMessage, Panel pnl3x3, Panel pnlDiagData, CheckBox chkFullDebug)
        {
            Observation objObservation = new Observation();
            List<string> colGrid = new List<string>();
            List<NearbyEntity> colNearbyEntities = new List<NearbyEntity>();
            LineOfSight objLineOfSight = new LineOfSight();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            
            try
            {
                if (worldState.number_of_observations_since_last_state > 0)
                {
                    // Get the Observation
                    objObservation = (Observation)Newtonsoft.Json.JsonConvert.DeserializeObject(worldState.observations[0].text, typeof(Observation));
                   
                    #region if (objObservation.floor3x3 != null)
                    if (objObservation.floor3x3 != null)
                    {
                        // Get the floor3x3
                        colGrid = objObservation.floor3x3;
                        // Diplay the Grid
                        Display3x3Grid.DisplayGrid(pnl3x3, colGrid);

                        if (chkFullDebug.Checked)
                        {
                            lstMessage.Items.Insert(0,
                                String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
                                colGrid[0], colGrid[1], colGrid[2], colGrid[3], colGrid[4],
                                colGrid[5], colGrid[6], colGrid[7], colGrid[8]));
                        }
                    }
                    #endregion

                    #region if (objObservation.NearbyEntities != null)
                    if (objObservation.NearbyEntities != null)
                    {
                        // Get the NearbyEntities
                        colNearbyEntities = objObservation.NearbyEntities;

                        if (chkFullDebug.Checked)
                        {
                            foreach (NearbyEntity objNearbyEntity in colNearbyEntities)
                            {
                                lstMessage.Items.Insert(0,
                                    String.Format("name={0} x={1} y={2} z={3}",
                                    objNearbyEntity.name,
                                    objNearbyEntity.x,
                                    objNearbyEntity.y,
                                    objNearbyEntity.z));
                            }
                        }
                    }
                    #endregion

                    #region if (objObservation.LineOfSight != null)
                    if (objObservation.LineOfSight != null)
                    {
                        // Get the LineOfSight
                        objLineOfSight = objObservation.LineOfSight;

                        Label lblLineOfSightName = (Label)pnlDiagData.Controls.Find("lblLineOfSightName", false)[0];
                        lblLineOfSightName.Text = String.Format("Name: {0}", objLineOfSight.type);
                        lblLineOfSightName.Invalidate();
                        lblLineOfSightName.Refresh();

                        Label lblLineOfSightX = (Label)pnlDiagData.Controls.Find("lblLineOfSightX", false)[0];
                        lblLineOfSightX.Text = String.Format("Distance from X: {0}", (Convert.ToDouble(objLineOfSight.x) - (objObservation.XPos)));
                        lblLineOfSightX.Invalidate();
                        lblLineOfSightX.Refresh();

                        Label lblLineOfSightY = (Label)pnlDiagData.Controls.Find("lblLineOfSightY", false)[0];
                        lblLineOfSightY.Text = String.Format("Distance from Y: {0}", (Convert.ToDouble(objLineOfSight.y) - (objObservation.YPos)));
                        lblLineOfSightY.Invalidate();
                        lblLineOfSightY.Refresh();

                        Label lblLineOfSightZ = (Label)pnlDiagData.Controls.Find("lblLineOfSightZ", false)[0];
                        lblLineOfSightZ.Text = String.Format("Distance from Z: {0}", (Convert.ToDouble(objLineOfSight.z) - (objObservation.ZPos)));
                        lblLineOfSightZ.Invalidate();
                        lblLineOfSightZ.Refresh();

                        if (chkFullDebug.Checked)
                        {
                            lstMessage.Items.Insert(0,
                            String.Format("hitType={0} inRange={1} type={2} variant={3} x={4} y={5} z={6} ",
                            objLineOfSight.hitType,
                            objLineOfSight.inRange,
                            objLineOfSight.type,
                            objLineOfSight.variant,
                            objLineOfSight.x,
                            objLineOfSight.y,
                            objLineOfSight.z));
                        }
                    }
                    #endregion

                }

                foreach (TimestampedReward reward in worldState.rewards)
                {
                    lstMessage.Items.Insert(0, String.Format("Summed reward: {0}", reward.getValue()));
                }

                foreach (TimestampedString error in worldState.errors)
                {
                    lstMessage.Items.Insert(0, String.Format("Error: {0}", error.text));
                }

                lstMessage.Refresh();
                
            }
            catch (Exception ex)
            {
                lstMessage.Items.Insert(0, ex.Message);
            }

            return objObservation;
        }
    }
}

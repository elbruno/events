// --------------------------------------------------------------------------------------------------
//  Copyright (c) 2016 Microsoft Corporation
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
//  associated documentation files (the "Software"), to deal in the Software without restriction,
//  including without limitation the rights to use, copy, modify, merge, publish, distribute,
//  sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all copies or
//  substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
//  NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// --------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using Microsoft.Research.Malmo;
using System.IO;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MalmoCSharpLauncher
{
    public partial class Main : Form
    {
        string strBaseLocation = "../../../SampleMissions/";
        string XMLFile = "";
        public Main()
        {
            InitializeComponent();
        }
       
        private void btnRun_Click(object sender, EventArgs e)
        {
            AgentHost agentHost = new AgentHost();
            try
            {
                agentHost.parse(new StringVector(Environment.GetCommandLineArgs()));
            }
            catch (Exception ex)
            {
                lstMessage.Items.Insert(0, String.Format("ERROR: {0}", ex.Message));
                lstMessage.Items.Insert(0, agentHost.getUsage());
                Environment.Exit(1);
            }
            if (agentHost.receivedArgument("help"))
            {
                lstMessage.Items.Insert(0, agentHost.getUsage());
                Environment.Exit(0);
            }

            // Clear debug box
            lstMessage.Items.Clear();

            // Load the .xml mission file and
            // Run the Mission
            switch (cbSelect.SelectedItem.ToString())
            {
                case "DefaultMission":
                    XMLFile = $"{strBaseLocation}DefaultMission.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    DefaultMission.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
                case "AvoidTheLava":
                    XMLFile = $"{strBaseLocation}AvoidTheLava.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    AvoidTheLava.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
                case "Tutorial4":
                    XMLFile = $"{strBaseLocation}Tutorial4.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    Tutorial4.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
                case "Maze_2":
                    XMLFile = $"{strBaseLocation}maze_2.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    maze2.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
                case "ObserveExampleOne":
                    XMLFile = $"{strBaseLocation}ObserveExampleOne.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    ObserveExampleOne.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
                case "TryToNavigateByVideo":
                    XMLFile = $"{strBaseLocation}TryToNavigateByVideo.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    TryToNavigateByVideo.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
                default:
                    XMLFile = $"{strBaseLocation}DefaultMission.xml";
                    LoadMinecraft(agentHost, XMLFile, lstMessage);
                    DefaultMission.Execute(agentHost, lstMessage, pnl3x3, pnlDiagData, pictureBoxMineCraft, chkFullDebug);
                    break;
            }   
        }

        #region private static void LoadMinecraft(AgentHost agentHost, string XMLFile, ListBox lstMessage)
        private static void LoadMinecraft(AgentHost agentHost, string XMLFile, ListBox lstMessage)
        {
            String strXMLFileContents = "";
            using (StreamReader sr = new StreamReader(XMLFile))
            {
                // Read the stream to a string, and write the string to the console.
                strXMLFileContents = sr.ReadToEnd();
            }

            MissionSpec mission = new MissionSpec(strXMLFileContents, false);
            mission.requestVideo(640, 480);

            MissionRecordSpec missionRecord = new MissionRecordSpec("./saved_data.tgz");
            missionRecord.recordCommands();
            missionRecord.recordMP4(20, 400000);
            missionRecord.recordRewards();
            missionRecord.recordObservations();

            try
            {
                agentHost.startMission(mission, missionRecord);
            }
            catch (Exception ex)
            {
                lstMessage.Items.Insert(0, String.Format("Error starting mission: {0}", ex.Message));
                Environment.Exit(1);
            }

            WorldState worldState;

            lstMessage.Items.Insert(0, "Waiting for the mission to start");
            do
            {
                Thread.Sleep(100);
                worldState = agentHost.getWorldState();

                foreach (TimestampedString error in worldState.errors)
                {
                    lstMessage.Items.Insert(0, String.Format("Error: {0}", error.text));
                }
            }
            while (!worldState.has_mission_begun);
        } 
        #endregion
    }
}

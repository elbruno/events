using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MalmoCSharpLauncher
{
    class Display3x3Grid
    {
        public static void DisplayGrid(Panel pnl3x3, List<string> colGrid)
        {
            // Get the PictureBoxs from the Panel
            PictureBox Box1 = (PictureBox)pnl3x3.Controls.Find("pictureBox1",false)[0];
            PictureBox Box2 = (PictureBox)pnl3x3.Controls.Find("pictureBox2", false)[0];
            PictureBox Box3 = (PictureBox)pnl3x3.Controls.Find("pictureBox3", false)[0];
            PictureBox Box4 = (PictureBox)pnl3x3.Controls.Find("pictureBox4", false)[0];
            PictureBox Box5 = (PictureBox)pnl3x3.Controls.Find("pictureBox5", false)[0];
            PictureBox Box6 = (PictureBox)pnl3x3.Controls.Find("pictureBox6", false)[0];
            PictureBox Box7 = (PictureBox)pnl3x3.Controls.Find("pictureBox7", false)[0];
            PictureBox Box8 = (PictureBox)pnl3x3.Controls.Find("pictureBox8", false)[0];
            PictureBox Box9 = (PictureBox)pnl3x3.Controls.Find("pictureBox9", false)[0];

            Box1.Image = Image.FromFile(GetImageLocation(colGrid[0]));
            Box1.Invalidate();
            Box1.Refresh();

            Box2.Image = Image.FromFile(GetImageLocation(colGrid[1]));
            Box2.Invalidate();
            Box2.Refresh();

            Box3.Image = Image.FromFile(GetImageLocation(colGrid[2]));
            Box3.Invalidate();
            Box3.Refresh();

            Box4.Image = Image.FromFile(GetImageLocation(colGrid[3]));
            Box4.Invalidate();
            Box4.Refresh();

            Box5.Image = Image.FromFile(GetImageLocation(colGrid[4]));
            Box5.Invalidate();
            Box5.Refresh();

            Box6.Image = Image.FromFile(GetImageLocation(colGrid[5]));
            Box6.Invalidate();
            Box6.Refresh();

            Box7.Image = Image.FromFile(GetImageLocation(colGrid[6]));
            Box7.Invalidate();
            Box7.Refresh();

            Box8.Image = Image.FromFile(GetImageLocation(colGrid[7]));
            Box8.Invalidate();
            Box8.Refresh();

            Box9.Image = Image.FromFile(GetImageLocation(colGrid[8]));
            Box9.Invalidate();
            Box9.Refresh();
        }

        private static string GetImageLocation(string strImage)
        {
            string strBaseLocation = @"../../../Images/";
            switch (strImage)
            {
                case "lava":
                case "flowing_lava":
                    return $"{strBaseLocation}lava.bmp";
                case "dirt":
                    return $"{strBaseLocation}dirt.bmp";
                case "grass":
                    return $"{strBaseLocation}grass.bmp";
                case "lapis":
                case "lapis_ore":
                case "lapis_block":
                    return $"{strBaseLocation}lapis.bmp";
                case "water":
                case "flowing_water":
                    return $"{strBaseLocation}water.bmp";
                case "sandstone":
                case "sand":
                case "soul_sand":
                case "red_sandstone":
                case "glass":
                    return $"{strBaseLocation}sand.bmp";
                case "obsidian":
                    return $"{strBaseLocation}obsidian.bmp";
                default:
                    return $"{strBaseLocation}cobble.bmp";
            }
        }
    }
}

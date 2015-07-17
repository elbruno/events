using System;

namespace WuaBleRs02.Drone
{
    public class RollingSpiderCharacteristicUuids
    {
        // definitions from Jessica https://github.com/robotika/jessica/blob/master/android/SampleGattAttributes.java

        public static Guid Parrot_AIF => new Guid("9a66fa1f-0800-9191-11e4-012d1540cb8e"); // handle 0x7F
        public static Guid Parrot_B01 => new Guid("9a66fb01-0800-9191-11e4-012d1540cb8e"); // + complete range to B1F (handle 0x92-0xEF)
        public static Guid Parrot_B0E_BC_BD => new Guid("9a66fb0e-0800-9191-11e4-012d1540cb8e"); // handle 0xBC , "Parrot - B0E/BC-BD"
        public static Guid Parrot_B1B_E3_E4 => new Guid("9a66fb1b-0800-9191-11e4-012d1540cb8e"); // handle 0xE3, "Parrot - B1B/E3-E4"
        public static Guid Parrot_B1C_E6_E7 => new Guid("9a66fb1c-0800-9191-11e4-012d1540cb8e"); // handle 0xE6 Parrot - B1C/E6-E7
        public static Guid Parrot_B1F => new Guid("9a66fb1f-0800-9191-11e4-012d1540cb8e");
        public static Guid Parrot_Battery_B0F_BF_C0 => new Guid("9a66fb0f-0800-9191-11e4-012d1540cb8e"); // handle 0xBF-0xC0, "Parrot Battery - B0F/BF-C0"
        public static Guid Parrot_D22 => new Guid("9a66fd22-0800-9191-11e4-012d1540cb8e"); // handle 0x112
        public static Guid Parrot_D23 => new Guid("9a66fd23-0800-9191-11e4-012d1540cb8e"); // handle 0x115
        public static Guid Parrot_D24 => new Guid("9a66fd24-0800-9191-11e4-012d1540cb8e"); // handle 0x118
        public static Guid Parrot_D52 => new Guid("9a66fd52-0800-9191-11e4-012d1540cb8e"); // handle 0x122
        public static Guid Parrot_D53 => new Guid("9a66fd53-0800-9191-11e4-012d1540cb8e"); // handle 0x125
        public static Guid Parrot_D54 => new Guid("9a66fd54-0800-9191-11e4-012d1540cb8e"); // handle 0x128
        public static Guid Parrot_DateTime => new Guid("9a66fa0b-0800-9191-11e4-012d1540cb8e");   // handle 0x43, 1:2014-10-29, 2:T223238+0100, 3:0x0
        public static Guid Parrot_EmergencyStop => new Guid("9a66fa0c-0800-9191-11e4-012d1540cb8e");   // handle 0x46, send("02 00 04 00")
        public static Guid Parrot_FC1 => new Guid("9a66ffc1-0800-9191-11e4-012d1540cb8e"); // handle 0x102
        public static Guid Parrot_InitCount1_20 => new Guid("9a66fa1e-0800-9191-11e4-012d1540cb8e"); // handle 0x7C
        public static Guid Parrot_PowerMotors => new Guid("9a66fa0a-0800-9191-11e4-012d1540cb8e");     // handle 0x40
        public static Guid Parrot_Stop => new Guid("9a66fa02-0800-9191-11e4-012d1540cb8e"); // !!! S T O P    !!!
        public static Guid Parrot_TourTheStairsParrotA01 => new Guid("9a66fa01-0800-9191-11e4-012d1540cb8e"); // + complete range to A1F (handle 0x22 - 0x7F)
    }
}
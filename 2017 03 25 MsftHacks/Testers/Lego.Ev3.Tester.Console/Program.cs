using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Threading.Tasks;


namespace Lego.Ev3.Tester.Console
{
	class Program
	{
		static Brick _brick;

		static void Main(string[] args)
		{	
			Task t = Test();
			t.Wait();
			System.Console.ReadKey();
		}

		static async Task Test()
		{
			//_brick = new Brick(new UsbCommunication());
			_brick = new Brick(new BluetoothCommunication("COM6"));
			//_brick = new Brick(new NetworkCommunication("192.168.2.237"));

			_brick.BrickChanged += _brick_BrickChanged;

			System.Console.WriteLine("Connecting...");
			await _brick.ConnectAsync();

			System.Console.WriteLine("Connected...Turning motor...");
			//await _brick.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.A, 0x50, 1000, false);

			System.Console.WriteLine("Motor turned...beeping...");
			await _brick.DirectCommand.PlayToneAsync(0x50, 5000, 500);

			System.Console.WriteLine("Beeped...done!");
		}

		static void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
		{
			System.Console.WriteLine(e.Ports[InputPort.One].SIValue);
		}
	}
}

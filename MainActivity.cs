using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using System.Text;

namespace robot
{
	[Activity (Label = "robot", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, ISensorEventListener
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			_sensorManager = (SensorManager)GetSystemService (Context.SensorService);
			_sensorTextView = FindViewById<TextView> (Resource.Id.textView1);
			_command = FindViewById<TextView> (Resource.Id.textView2);
			_status = FindViewById<TextView> (Resource.Id.textView3);
			connectButton = FindViewById<Button> (Resource.Id.button1);
			_sensitivity = FindViewById<SeekBar> (Resource.Id.seekBar1);

			_status.Text = "Durum: Bağlantı yok";
			_status.SetTextColor (Android.Graphics.Color.Red);
						connectButton.Click += delegate {                 // baglan butonu implementasyonu.. Tıklandıgında baglanmaya calıs
				if(!communication.connection){
				if (communication.connectToServer ()) {
					_status.Text = "Durum: Bağlandı";                                 // ve duruma gore bilgi ver
					_status.SetTextColor (Android.Graphics.Color.Green);
						connectButton.Text="Bağlantıyı Sonlandır";
				} else {
					_status.Text = "Durum: Bağlantı yok";
					_status.SetTextColor (Android.Graphics.Color.Red);
				}
				}
				else
				{
					communication.disconnect();
					_status.Text = "Durum: Bağlantı yok";
					_status.SetTextColor (Android.Graphics.Color.Red);
					connectButton.Text="Bağlan";
				}

			};
			_sensitivity.Progress = (int)sens*10;
			_sensitivity.ProgressChanged+=(object sender, SeekBar.ProgressChangedEventArgs e) => {
				if(e.FromUser)
					this.sens=((double)e.Progress)/10.0;
				};


		}

		private SensorManager _sensorManager = null;
		private TextView _sensorTextView = null;		
		private static readonly object _syncLock = new object();
		public TextView _status=null;
		private TextView _command=null;
		private Communication communication=new Communication();
		private Button connectButton=null;
		private SeekBar _sensitivity=null;

		private String forward="1";       
		private String back="2";          /* Robota gönderilecek verilerin tutulduğu değiskenler */
		private String right="3";       
		private String left="4";

		private double sens=2;          // duyarlilik


		public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
		{
			// tek sebebi implement ettigimiz interface'in methodu. hicbir is yaptirmiyoruz.
		}

		public void OnSensorChanged(SensorEvent e)   // degisiklik oldukca uygun komutlari gonder
		{                                                      
			lock (_syncLock)
			{
				
				var text = new StringBuilder("x = ")
					.Append(e.Values[0])
					.Append(", y=")
					.Append(e.Values[1])
					.Append(", z=")
					.Append(e.Values[2]);
				_sensorTextView.Text = text.ToString();

				if ((e.Values [0] > sens || e.Values [0] < -sens) && (e.Values [1] > sens || e.Values [1] < -sens)) { // eger iki komut aynı anda gelirse
					if (Math.Abs (e.Values [0]) > Math.Abs (e.Values [1])) {         // en yuksek degeri bul
						if (e.Values [0] < 0) {                                                                 // ve buldugunu yaz
							communication.writeData (right);
							_command.Text = "Komut: Sağ";
						} else {
							communication.writeData (left);
							_command.Text = "Komut: Sol";
						}
					} else {
						if (e.Values [1] < 0) {
							communication.writeData (forward);
							_command.Text = "Komut: ileri";
						} else {
							communication.writeData (back);
							_command.Text = "Komut: Geri";
						}
					}
				} else {
					if (Math.Abs (e.Values [0]) > sens) {           // komut nereden gelmisse uygun olani yaz
						if (e.Values [0] < 0) {
							communication.writeData (right);
							_command.Text = "Komut: Sağ";
						} else {
							communication.writeData (left);
							_command.Text = "Komut: Sol";
						}
					}
					if (Math.Abs (e.Values [1]) > sens) {
						if (e.Values [1] < -sens) {
							communication.writeData (forward);
							_command.Text = "Komut: ileri";
						} else {
							communication.writeData (back);
							_command.Text = "Komut: Geri";
						}
					}
				}
				if (Math.Abs (e.Values [0]) < sens && Math.Abs (e.Values [1]) < sens)
					_command.Text = "Komut: - ";
			}
		}

		protected override void OnResume()  // degisimleri dinlemeye devam etmesi icin
		{
			base.OnResume ();
			_sensorManager.RegisterListener (this, _sensorManager.GetDefaultSensor (SensorType.Accelerometer), SensorDelay.Ui);
		}

		protected override void OnPause()  // uygulama kapaliyken dinlememesi, gereksiz batarya kullanimini onlemek icin
		{
			base.OnPause ();
			_sensorManager.UnregisterListener (this);
		}


	}
}



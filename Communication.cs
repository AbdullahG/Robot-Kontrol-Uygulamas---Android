using System;
using System.Net.Sockets;
using System.Text;

namespace robot
{
	public class Communication
	{
		public Boolean connection=false;
		TcpClient client=null; 

		private int port=0;
		private String ip_adress=null;
		private NetworkStream streamForData=null;        // veri gondermek icin

		public Communication ()
		{
			client = new TcpClient ();
		}

		public Boolean connectToServer ()
		{
			try {
				client.Connect (ip_adress, port);  // bu ip ve port'a baglan

				streamForData = client.GetStream ();  // veri gondermek icin yolu getir

				this.connection = true;
				return true;
			} catch {
				return false;
			}

		}

		public void disconnect()
		{
			this.connection = false;
			streamForData = null;
		}

		public String Ip_adress {
			get {
				return ip_adress;
			}
			set {
				ip_adress = value;
			}
		}

		public int Port {
			get {
				return port;
			}
			set {
				port = value;
			}
		}

		public void writeData(String data_string)
		{
			if (this.connection) {
				byte[] data = Encoding.ASCII.GetBytes (data_string);

				streamForData.Write (data, 0, data.Length);
			}
		}

	}
}


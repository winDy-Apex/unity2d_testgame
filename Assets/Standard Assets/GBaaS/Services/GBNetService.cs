using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GBaaS.io.Services;
using GBaaS.io.Objects;
using GBaaS.io.Utils;
using System.Threading;
using System.Collections.Generic;

namespace GBaaS.io
{
	class GBNetService : GBService<GBNetService>
	{
		public GBNetService () {}

		public GBaaSApiHandler _handler = null;

		private Socket _socket;
		private byte[] _getbyte = new byte[1024];
		private byte[] _setbyte = new byte[1024];

		public void SetHandler(GBaaSApiHandler handler) {
			_handler = handler;
		}

		private bool IsAsync() {
			return (_handler != null);
		}

		public bool ReceiveDataThread() {
			while(true)
			{
				//Console.WriteLine("...In ReceiveDataThread...");
				int getValueLength = _socket.Receive(_getbyte,0,_getbyte.Length,SocketFlags.None);
				string getstring = Encoding.ASCII.GetString(_getbyte,0,getValueLength+1);
				//Console.WriteLine(">>수신된 데이터 :{0} | 길이{1}" , getstring , getstring.Length);
				_handler.OnReceiveData(getstring);
				_getbyte = new byte[1024];

				System.Threading.Thread.Sleep(90);
			}
		}

		public bool SessionIn(string userName, string appID) {
			StringBuilder sbJsonRequest = new StringBuilder();
			sbJsonRequest.AppendFormat("\"PacketType\":{0},", 11);
			sbJsonRequest.AppendFormat("\"Name\":\"{0}\",", userName);
			sbJsonRequest.AppendFormat("\"AppID\":\"{0}\",", appID);

			string jsonPacket = "#" + (sbJsonRequest.Length + 2).ToString() + "{" + sbJsonRequest.ToString().TrimEnd(',') + "}";

			Console.WriteLine(jsonPacket);

			return SendData(jsonPacket) > 0;
		}

		public bool LobbyIn(string lobbyIP, int lobbyPort, string appID) {
			if (_socket == null || !_socket.Connected) {
				Connect(lobbyIP, lobbyPort);
			}

			// LOBBY_IN
			StringBuilder sbJsonRequest = new StringBuilder();
			sbJsonRequest.AppendFormat("\"PacketType\":{0},", 13);
			sbJsonRequest.AppendFormat("\"Name\":\"{0}\",", GBUserService.Instance.GetLoginName());
			sbJsonRequest.AppendFormat("\"AppID\":\"{0}\",", appID);
			sbJsonRequest.AppendFormat("\"LobbyID\":{0},", 1);
			sbJsonRequest.AppendFormat("\"RoomListSubscribe\":{0},", "false");

			string jsonPacket = "#" + (sbJsonRequest.Length + 2).ToString() + "{" + sbJsonRequest.ToString().TrimEnd(',') + "}";

			//Console.WriteLine(jsonPacket);

			return SendData(jsonPacket) > 0;
		}

		public bool RoomIn(int roomID, string title, string tag, string password, int maxUser) {
			if (_socket == null || !_socket.Connected) {
				return false;
			}

			// ROOM_IN
			StringBuilder sbJsonRequest = new StringBuilder();
			sbJsonRequest.AppendFormat("\"PacketType\":{0},", 21);
			sbJsonRequest.AppendFormat("\"RoomID\":{0},", roomID);
			sbJsonRequest.AppendFormat("\"Title\":\"{0}\",", title);
			sbJsonRequest.AppendFormat("\"Tag\":\"{0}\",", tag);
			sbJsonRequest.AppendFormat("\"Password\":\"{0}\",", password);
			sbJsonRequest.AppendFormat("\"MaxUser\":{0},", maxUser);

			string jsonPacket = "#" + (sbJsonRequest.Length + 2).ToString() + "{" + sbJsonRequest.ToString().TrimEnd(',') + "}";

			//Console.WriteLine(jsonPacket);

			return SendData(jsonPacket) > 0;
		}

		/*
		private bool SessionInThread(
			string message, string scheduleDate, string deviceIds, string groupPaths, string userNames, 
			PushSendType sendType, PushScheduleType scheduleType) {
			GBPushMessageObject pushMessage = new GBPushMessageObject {
				message = message,
				scheduleDate = scheduleDate,
				deviceIds = deviceIds,
				groupPaths = groupPaths,
				userNames = userNames,
				sendType = sendType.ToString(),
				scheduleType = scheduleType.ToString()
			};

			if (IsAsync()) {
				_handler.OnSendMessage(pushMessage.Save());
			} else {
				return pushMessage.Save();;
			}

			return false;
		}
		*/

		public List<Objects.GBRoom> GetRoomList() {
			StringBuilder sbJsonRequest = new StringBuilder();
			sbJsonRequest.AppendFormat("\"PacketType\":{0},", 19);

			string jsonPacket = "#" + (sbJsonRequest.Length + 2).ToString() + "{" + sbJsonRequest.ToString().TrimEnd(',') + "}";

			//Console.WriteLine(jsonPacket);

			SendData(jsonPacket);

			return default(List<Objects.GBRoom>);
		}

		public bool ChannelSend(string data, int dataType=0, bool echo=true) {
			if (_socket == null || !_socket.Connected) {
				return false;
			}

			// CHANNEL_SEND
			StringBuilder sbJsonRequest = new StringBuilder();
			sbJsonRequest.AppendFormat("\"PacketType\":{0},", 41);
			sbJsonRequest.AppendFormat("\"Data\":\"{0}\",", data);
			sbJsonRequest.AppendFormat("\"DataType\":{0},", dataType);
			sbJsonRequest.AppendFormat("\"Echo\":{0},", echo.ToString().ToLower());

			string jsonPacket = "#" + (sbJsonRequest.Length + 2).ToString() + "{" + sbJsonRequest.ToString().TrimEnd(',') + "}";

			//Console.WriteLine(jsonPacket);

			return SendData(jsonPacket) > 0;
		}

		public bool Connect(string IP, int port) {
			IPAddress serverIP = IPAddress.Parse(IP);
			IPEndPoint serverEndPoint = new IPEndPoint(serverIP,port);

			_socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			_socket.Connect(serverEndPoint);

			if (_socket.Connected) {
				Thread workerThread = new Thread(() => this.ReceiveDataThread());
				workerThread.Start();
				return true;
			}

			return false;
		}

		public int SendData(string jsonPacket) {
			if (!_socket.Connected) {
				return -1;
			}

			_setbyte = Encoding.ASCII.GetBytes(jsonPacket);
			return _socket.Send(_setbyte, 0, _setbyte.Length, SocketFlags.None);
		}
	}
}

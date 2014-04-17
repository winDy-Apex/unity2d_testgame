using System;
using GBaaS.io.Services;
using GBaaS.io.Objects;
using GBaaS.io.Utils;
using System.Threading;

namespace GBaaS.io
{
	class GBPushService : GBService<GBPushService>
	{
		public GBPushService () {}

		public GBaaSApiHandler _handler = null;

		public void SetHandler(GBaaSApiHandler handler) {
			_handler = handler;
		}

		private bool IsAsync() {
			return (_handler != null);
		}

		public bool SendMessage(
			string message, string scheduleDate, string deviceIds, string groupPaths, string userNames, 
			PushSendType sendType, PushScheduleType scheduleType) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.SendMessageThread(message, scheduleDate, deviceIds, groupPaths, userNames, sendType, scheduleType));
				workerThread.Start();
				return false;
			} else {
				return this.SendMessageThread(message, scheduleDate, deviceIds, groupPaths, userNames, sendType, scheduleType);
			}
		}

		private bool SendMessageThread(
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

		public bool RegisterDevice(
			string deviceModel, string deviceOSVersion, string devicePlatform, string registeration_id) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.RegisterDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registeration_id));
				workerThread.Start();
				return false;
			} else {
				return this.RegisterDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registeration_id);
			}
		}

		private bool RegisterDeviceThread(
			string deviceModel, string deviceOSVersion, string devicePlatform, string registration_id) {

			deviceModel = deviceModel.Replace(" ", "_");
			deviceOSVersion = deviceOSVersion.Replace(" ", "_");

			// 등록된 디바이스인지 확인
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/devices?registration_id=" + registration_id, HttpHelper.RequestTypes.Get, "");

			bool isRegistered = HttpHelper.Instance.CheckSuccess(rawResults);

			if (isRegistered) {
				var registeredDevices = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
				foreach (var device in registeredDevices) {
					if (IsAsync()) {
						_handler.OnRegisterDevice(isRegistered);
					} else {
						return isRegistered;
					}
				}
			}

			// 등록안된 디바이스인경우 등록
			GBDeviceRegisterObject deviceRegister = new GBDeviceRegisterObject {
				deviceModel = deviceModel,
				deviceOSVersion = deviceOSVersion,
				devicePlatform = devicePlatform,
				registration_id = registration_id
			};

			bool result = deviceRegister.Save();
			if (result == false) {
				if (IsAsync()) {
					_handler.OnRegisterDevice(result);
				} else {
					return result;
				}
			}

			// 등록된 디바이스 UUID 정보 가져오기
			rawResults = GBRequestService.Instance.PerformRequest<string>("/devices?registration_id=" + registration_id, HttpHelper.RequestTypes.Get, "");

			isRegistered = HttpHelper.Instance.CheckSuccess(rawResults);

			string deviceID = "";

			if (isRegistered) {
				var registeredDevices = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
				foreach (var device in registeredDevices) {
					deviceID = (device["uuid"] ?? "").ToString();
					if (deviceID.Length > 0)
						break;
				}
			}

			if (IsAsync()) {
				_handler.OnRegisterDevice(ConnectDeviceToUser(deviceID));
			} else {
				return ConnectDeviceToUser(deviceID);
			}

			return false;
		}

		private bool ConnectDeviceToUser(string deviceID) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>(
				"/users/" + GBUserService.Instance.GetLoginName() + "/devices/" + deviceID, HttpHelper.RequestTypes.Post, "");

			return HttpHelper.Instance.CheckSuccess(rawResults);
		}
	}
}

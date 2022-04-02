using Dimension.Domain;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.Call;
using ManageLiteAV;

namespace DimensionClient.Common
{
    public class CallViewManager : ITRTCCloudCallback, ITRTCLogCallback
    {
        private readonly string _roomID;
        private readonly GetRoomKeyModel _roomKey;
        private readonly CallType _callType;
        private readonly bool _houseOwner;

        private ITRTCCloud cloud;
        public List<CallViewDataModel> CallViews { get; private set; } = new List<CallViewDataModel>();

        public CallViewManager(string roomID, GetRoomKeyModel roomKey, CallType callType, List<string> member, bool houseOwner = false)
        {
            _roomID = roomID;
            _roomKey = roomKey;
            _callType = callType;
            _houseOwner = houseOwner;

            foreach (string item in member)
            {
                CallViewDataModel callVideoData = new()
                {
                    UserID = item
                };
                CallViews.Add(callVideoData);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            Task.Run(() =>
            {
                ClassHelper.ToCall(_callType, true);
                cloud = ITRTCCloud.getTRTCShareInstance();
                TRTCParams tRTCParams = new()
                {
                    sdkAppId = ClassHelper.callAppID,
                    userId = ClassHelper.UserID,
                    userSig = _roomKey.UserSig,
                    privateMapKey = _roomKey.PrivateMapKey,
                    strRoomId = _roomID
                };

                cloud.addCallback(this);

                cloud.enterRoom(ref tRTCParams, _callType == CallType.Video ? TRTCAppScene.TRTCAppSceneVideoCall : TRTCAppScene.TRTCAppSceneAudioCall);
            });
        }

        /// <summary>
        /// 卸载
        /// </summary>
        public void UnInitialize()
        {
            Task.Run(() =>
            {
                ClassHelper.ToCall(_callType, false);
                cloud.exitRoom();
                ITRTCCloud.destroyTRTCShareInstance();
                cloud.Dispose();
                cloud = null;
                if (_houseOwner)
                {
                    CallService.DissolutionRoom();
                }
                ClassHelper.CallViewManager = null;
            });
        }

        /// <summary>
        /// 摄像头开关
        /// </summary>
        /// <param name="state">开关</param>
        public void CameraSwitch(bool state)
        {
            if (CallViews.FirstOrDefault(item => item.UserID == ClassHelper.UserID) is CallViewDataModel callVideoData)
            {
                if (callVideoData.IsVideo != state)
                {
                    callVideoData.IsVideo = state;
                    if (state)
                    {
                        cloud.startLocalPreview(IntPtr.Zero);
                    }
                    else
                    {
                        cloud.stopLocalPreview();
                    }
                    cloud.muteLocalVideo(TRTCVideoStreamType.TRTCVideoStreamTypeBig, !state);
                }
            }
        }

        /// <summary>
        /// 麦克风开关
        /// </summary>
        /// <param name="state">开关</param>
        public void MicrophoneSwitch(bool state)
        {
            if (CallViews.FirstOrDefault(item => item.UserID == ClassHelper.UserID) is CallViewDataModel callVideoData)
            {
                if (callVideoData.IsAudio != state)
                {
                    callVideoData.IsAudio = state;
                    cloud.muteLocalAudio(!state);
                }
            }
        }

        #region ITRTCCloudCallback
        public void onAudioDeviceCaptureVolumeChanged(uint volume, bool muted)
        {

        }

        public void onAudioDevicePlayoutVolumeChanged(uint volume, bool muted)
        {

        }

        public void onAudioEffectFinished(int effectId, int code)
        {

        }

        public void onCameraDidReady()
        {

        }

        public void onConnectionLost()
        {

        }

        public void onConnectionRecovery()
        {

        }

        public void onConnectOtherRoom(string userId, TXLiteAVError errCode, string errMsg)
        {

        }

        public void onDeviceChange(string deviceId, TRTCDeviceType type, TRTCDeviceState state)
        {

        }

        public void onDisconnectOtherRoom(TXLiteAVError errCode, string errMsg)
        {

        }

        public void onEnterRoom(int result)
        {
            if (result >= 0)
            {
                if (CallViews.FirstOrDefault(item => item.UserID == ClassHelper.UserID) is CallViewDataModel callVideoData)
                {
                    callVideoData.IsEnter = true;

                    callVideoData.IsAudio = true;
                    cloud.startLocalAudio(TRTCAudioQuality.TRTCAudioQualityDefault);

                    if (_callType == CallType.Video)
                    {
                        TRTCRenderParams renderParams = new()
                        {
                            rotation = TRTCVideoRotation.TRTCVideoRotation0,
                            fillMode = TRTCVideoFillMode.TRTCVideoFillMode_Fit,
                            mirrorType = TRTCVideoMirrorType.TRTCVideoMirrorType_Disable
                        };
                        cloud.setLocalRenderParams(ref renderParams);

                        callVideoData.IsVideo = true;
                        cloud.startLocalPreview(IntPtr.Zero);
                        cloud.setLocalVideoRenderCallback(TRTCVideoPixelFormat.TRTCVideoPixelFormat_BGRA32, TRTCVideoBufferType.TRTCVideoBufferType_Buffer, callVideoData);
                    }
                }
                if (_houseOwner)
                {
                    CallService.NotifyRoommate();
                }
            }
        }

        public void onExitRoom(int reason)
        {

        }

        public void onFirstAudioFrame(string userId)
        {

        }

        public void onFirstVideoFrame(string userId, TRTCVideoStreamType streamType, int width, int height)
        {

        }

        public void onLocalRecordBegin(int errCode, string storagePath)
        {

        }

        public void onLocalRecordComplete(int errCode, string storagePath)
        {

        }

        public void onLocalRecording(int duration, string storagePath)
        {

        }

        public void onMicDidReady()
        {

        }

        public void onMissCustomCmdMsg(string userId, int cmdId, int errCode, int missed)
        {

        }

        public void onNetworkQuality(TRTCQualityInfo localQuality, TRTCQualityInfo[] remoteQuality, uint remoteQualityCount)
        {

        }

        public void onPlayBGMBegin(TXLiteAVError errCode)
        {

        }

        public void onPlayBGMComplete(TXLiteAVError errCode)
        {

        }

        public void onPlayBGMProgress(uint progressMS, uint durationMS)
        {

        }

        public void onRecvCustomCmdMsg(string userId, int cmdID, uint seq, byte[] msg, uint msgSize)
        {

        }

        public void onRecvSEIMsg(string userId, byte[] message, uint msgSize)
        {

        }

        public void onRemoteUserEnterRoom(string userId)
        {
            if (CallViews.FirstOrDefault(item => item.UserID == userId) is CallViewDataModel callVideoData)
            {
                callVideoData.IsEnter = true;
            }
        }

        public void onRemoteUserLeaveRoom(string userId, int reason)
        {
            if (CallViews.FirstOrDefault(item => item.UserID == userId) is CallViewDataModel callVideoData)
            {
                callVideoData.IsEnter = false;
            }
            if (!CallViews.Any(item => item.UserID != ClassHelper.UserID && item.IsEnter != false))
            {
                UnInitialize();
            }
        }

        public void onRemoteVideoStatusUpdated(string userId, TRTCVideoStreamType streamType, TRTCAVStatusType status, TRTCAVStatusChangeReason reason, IntPtr extrainfo)
        {

        }

        public void onScreenCaptureCovered()
        {

        }

        public void onScreenCapturePaused(int reason)
        {

        }

        public void onScreenCaptureResumed(int reason)
        {

        }

        public void onScreenCaptureStarted()
        {

        }

        public void onScreenCaptureStoped(int reason)
        {

        }

        public void onSendFirstLocalAudioFrame()
        {

        }

        public void onSendFirstLocalVideoFrame(TRTCVideoStreamType streamType)
        {

        }

        public void onSetMixTranscodingConfig(int errCode, string errMsg)
        {

        }

        public void onSnapshotComplete(string userId, TRTCVideoStreamType type, byte[] data, uint length, uint width, uint height, TRTCVideoPixelFormat format)
        {

        }

        public void onSpeedTest(TRTCSpeedTestResult currentResult, uint finishedCount, uint totalCount)
        {

        }

        public void onStartPublishCDNStream(int errCode, string errMsg)
        {

        }

        public void onStartPublishing(int errCode, string errMsg)
        {

        }

        public void onStatistics(TRTCStatistics statis)
        {

        }

        public void onStopPublishCDNStream(int errCode, string errMsg)
        {

        }

        public void onStopPublishing(int errCode, string errMsg)
        {

        }

        public void onSwitchRole(TXLiteAVError errCode, string errMsg)
        {

        }

        public void onSwitchRoom(TXLiteAVError errCode, string errMsg)
        {

        }

        public void onTestMicVolume(uint volume)
        {

        }

        public void onTestSpeakerVolume(uint volume)
        {

        }

        public void onTryToReconnect()
        {

        }

        public void onUserAudioAvailable(string userId, bool available)
        {
            if (CallViews.FirstOrDefault(item => item.UserID == userId) is CallViewDataModel callVideoData)
            {
                callVideoData.IsAudio = available;
            }
        }

        public void onUserEnter(string userId)
        {

        }

        public void onUserExit(string userId, int reason)
        {

        }

        public void onUserSubStreamAvailable(string userId, bool available)
        {

        }

        public void onUserVideoAvailable(string userId, bool available)
        {
            if (CallViews.FirstOrDefault(item => item.UserID == userId) is CallViewDataModel callVideoData)
            {
                callVideoData.IsVideo = available;
                if (available)
                {
                    cloud.startRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig, IntPtr.Zero);
                    cloud.setRemoteVideoRenderCallback(userId, TRTCVideoPixelFormat.TRTCVideoPixelFormat_BGRA32, TRTCVideoBufferType.TRTCVideoBufferType_Buffer, callVideoData);
                }
                else
                {
                    cloud.stopRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig);
                    cloud.setRemoteVideoRenderCallback(userId, TRTCVideoPixelFormat.TRTCVideoPixelFormat_BGRA32, TRTCVideoBufferType.TRTCVideoBufferType_Buffer, null);
                }
            }
        }

        public void onUserVoiceVolume(TRTCVolumeInfo[] userVolumes, uint userVolumesCount, uint totalVolume)
        {

        }

        public void onWarning(TXLiteAVWarning warningCode, string warningMsg, IntPtr arg)
        {

        }

        public void onError(TXLiteAVError errCode, string errMsg, IntPtr arg)
        {
            ClassHelper.MessageAlert(ClassHelper.MainWindow.GetType(), 3, errMsg);
        }
        #endregion

        #region ITRTCLogCallback
        public void onLog(string log, TRTCLogLevel level, string module)
        {

        }
        #endregion
    }
}

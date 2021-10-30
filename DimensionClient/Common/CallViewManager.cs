using DimensionClient.Library.CustomControls;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.Call;
using ManageLiteAV;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DimensionClient.Common
{
    public class CallViewManager : ITRTCCloudCallback, ITRTCVideoRenderCallback, ITRTCLogCallback
    {
        private readonly string _roomID;
        private readonly string _userSig;
        private readonly ClassHelper.CallType _callType;
        private readonly bool _houseOwner;

        private ITRTCCloud cloud;
        public Dictionary<string, CallVideoImage> Video { get; private set; } = new Dictionary<string, CallVideoImage>();

        public CallViewManager(string roomID, string userSig, ClassHelper.CallType callType, List<string> member, bool houseOwner = false)
        {
            _roomID = roomID;
            _userSig = userSig;
            _callType = callType;
            _houseOwner = houseOwner;

            ClassHelper.Dispatcher.Invoke(delegate
            {
                foreach (string item in member)
                {
                    Video.Add(item, new CallVideoImage(new CallVideoImageViewModel()));
                }
            });
        }

        public void Initialize()
        {
            CallService.ReplyCall(_roomID, true);

            cloud = ITRTCCloud.getTRTCShareInstance();
            TRTCParams tRTCParams = new()
            {
                sdkAppId = ClassHelper.callAppID,
                userId = ClassHelper.UserID,
                userSig = _userSig,
                strRoomId = _roomID
            };

            cloud.addCallback(this);

            cloud.setLocalVideoRenderCallback(TRTCVideoPixelFormat.TRTCVideoPixelFormat_BGRA32, TRTCVideoBufferType.TRTCVideoBufferType_Buffer, this);

            cloud.enterRoom(ref tRTCParams, _callType == ClassHelper.CallType.Video ? TRTCAppScene.TRTCAppSceneVideoCall : TRTCAppScene.TRTCAppSceneAudioCall);
        }

        public void UnInitialize()
        {
            cloud.exitRoom();
            ITRTCCloud.destroyTRTCShareInstance();
            cloud.Dispose();
            cloud = null;
            if (_houseOwner)
            {
                CallService.DissolutionRoom();
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
                TRTCRenderParams renderParams = new()
                {
                    rotation = TRTCVideoRotation.TRTCVideoRotation0,
                    fillMode = TRTCVideoFillMode.TRTCVideoFillMode_Fit,
                    mirrorType = TRTCVideoMirrorType.TRTCVideoMirrorType_Disable
                };
                cloud.setLocalRenderParams(ref renderParams);
                cloud.startLocalPreview(IntPtr.Zero);

                cloud.startLocalAudio(TRTCAudioQuality.TRTCAudioQualityDefault);

                if (_houseOwner)
                {
                    CallService.NotifyRoommate();
                }
            }
        }

        public void onError(TXLiteAVError errCode, string errMsg, IntPtr arg)
        {
            ClassHelper.MessageAlert(ClassHelper.MainWindow.GetType(), 1, errMsg);
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

        }

        public void onRemoteUserLeaveRoom(string userId, int reason)
        {

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
            if (available)
            {
                cloud.startRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig, IntPtr.Zero);
                cloud.setRemoteVideoRenderCallback(userId, TRTCVideoPixelFormat.TRTCVideoPixelFormat_BGRA32, TRTCVideoBufferType.TRTCVideoBufferType_Buffer, this);
            }
        }

        public void onUserVoiceVolume(TRTCVolumeInfo[] userVolumes, uint userVolumesCount, uint totalVolume)
        {

        }

        public void onWarning(TXLiteAVWarning warningCode, string warningMsg, IntPtr arg)
        {

        }
        #endregion

        #region ITRTCVideoRenderCallback
        public void onRenderVideoFrame(string userId, TRTCVideoStreamType streamType, TRTCVideoFrame frame)
        {
            try
            {
                if (Video.TryGetValue(string.IsNullOrEmpty(userId) ? ClassHelper.UserID : userId, out CallVideoImage videoImage))
                {
                    lock (videoImage)
                    {
                        if (videoImage.CallVideoData.Writeable == null)
                        {
                            videoImage.Dispatcher.Invoke(delegate
                            {
                                videoImage.CallVideoData.Writeable = new WriteableBitmap((int)frame.width, (int)frame.height, 96, 96, PixelFormats.Pbgra32, null);
                                videoImage.CallVideoData.Int32Rect = new Int32Rect(0, 0, (int)frame.width, (int)frame.height);
                            });
                        }
                        videoImage.Dispatcher.Invoke(delegate
                        {
                            videoImage.CallVideoData.Writeable.Lock();
                            Marshal.Copy(frame.data, 0, videoImage.CallVideoData.Writeable.BackBuffer, frame.data.Length);
                            videoImage.CallVideoData.Writeable.AddDirtyRect(videoImage.CallVideoData.Int32Rect);
                            videoImage.CallVideoData.Writeable.Unlock();
                        });
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // 任务取消，不算异常。
            }
        }
        #endregion

        #region ITRTCLogCallback
        public void onLog(string log, TRTCLogLevel level, string module)
        {

        }
        #endregion
    }
}

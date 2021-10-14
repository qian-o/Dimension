using DimensionClient.Component.Pages;
using DimensionClient.Component.Windows;
using DimensionClient.Models;
using DimensionClient.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using static DimensionClient.Models.DisplayDevice;

namespace DimensionClient.Common
{
    public delegate void MessageEvent(int messageType, string message);
    public delegate void NotificationEvent(string title, string message);
    public delegate void AccordingMaskEvent(bool show, bool loading);
    public delegate void RouteEvent(ClassHelper.PageType pageName);
    public delegate void DataPassing(object data);

    public static class ClassHelper
    {
        #region 常量
        // 服务器版本
        public const string serviceVersion = "1.0";
        // 服务器地址 ( http://47.96.133.119, http://localhost:5000 )
        public const string servicePath = "http://47.96.133.119";
        // 客户端标识
        public static readonly UseDevice device = UseDevice.Client;
        // 程序启动目录
        public static readonly string programPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        // 程序缓存目录
        public static readonly string cachePath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Cache");
        // 区域设置
        public static readonly CultureInfo cultureInfo = new("zh-cn");
        // 手机号正则验证
        public const string phoneVerify = @"^1[0-9]{10}$";
        // 邮箱正则验证
        public const string emailVerify = @"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";
        public const uint wpSystemMenu = 0x02;
        public const uint wmSystemMenu = 0xa4;
        public static readonly CommonViewModel commonView = new();
        // AES私人密钥
        public const string privateKey = "wangxi1234567890";
        public const int currentSettings = -1;
        public const int registrySettings = -2;
        #endregion

        #region 变量
        // 用户ID
        public static string UserID { get; set; }
        // 令牌
        public static string Token { get; set; }
        // 程序主线程
        public static Dispatcher Dispatcher { get; set; }
        // 登录窗体
        public static LoginWindow LoginWindow { get; set; }
        // 主窗体
        public static MainWindow MainWindow { get; set; }
        // 选中的聊天好友ID
        public static string ChatFriendID { get; set; }
        // 选中的联系人好友ID
        public static string ContactPersonFriendID { get; set; }
        #endregion

        #region 枚举
        // 使用设备
        public enum UseDevice
        {
            Phone,
            Web,
            Client
        }
        // MessageBox模式
        public enum MessageBoxType
        {
            Inform,
            Select
        }
        // MessageBox关闭方式
        public enum MessageBoxCloseType
        {
            Close,
            Left,
            Right
        }
        // SignalR消息类别
        public enum HubMessageType
        {
            // string title, string message
            Notification,
            // string friendID, bool online
            FriendOnline,
            // string friendID
            NewFriend,
            // string sort, string friendID, bool state ( true 添加 false 删除 )
            FriendChanged,
            // bool online
            OnlineStatus,
            // string friendID
            RemarkInfoChanged,
            // string friendID
            ChatColumnChanged,
            // string chatID
            NewMessage
        }
        // Page类型
        public enum PageType
        {
            MessageCenterPage,
            ContactPersonPage
        }
        // 新朋友类别
        public enum NewFriendType
        {
            Add,
            Verify
        }
        // 消息类别
        public enum MessageType
        {
            Text,
            Voice,
            File,
            VoiceTalk,
            VideoTalk
        }
        // 文件类型
        public enum FileType
        {
            Image,
            Word,
            Excel,
            PPT,
        }
        // 屏幕方向
        public enum Orientation
        {
            Angle0,
            Angle180,
            Angle270,
            Angle90
        }
        // 监控选项
        public enum MonitorOptions : uint
        {
            MONITORDEFAULTTONULL = 0x00000000,
            MONITORDEFAULTTOPRIMARY = 0x00000001,
            MONITORDEFAULTTONEAREST = 0x00000002
        }
        // Dpi类型
        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }
        #endregion

        #region 事件
        // 消息提醒
        private static MessageEvent GetMessage;
        public static event MessageEvent MessageHint
        {
            add
            {
                GetMessage += value;
            }
            remove
            {
                GetMessage -= value;
            }
        }
        // 显示蒙版
        private static AccordingMaskEvent GetAccording;
        public static event AccordingMaskEvent AccordingMask
        {
            add
            {
                GetAccording += value;
            }
            remove
            {
                GetAccording -= value;
            }
        }
        // 改变路由
        private static RouteEvent GetRouted;
        public static event RouteEvent RoutedChanged
        {
            add
            {
                GetRouted += value;
            }
            remove
            {
                GetRouted -= value;
            }
        }
        // 系统通知
        private static NotificationEvent GetNotification;
        public static event NotificationEvent NotificationHint
        {
            add
            {
                GetNotification += value;
            }
            remove
            {
                GetNotification -= value;
            }
        }
        // 类直接数据传递
        private static DataPassing GetDataPassing;
        public static event DataPassing DataPassingChanged
        {
            add
            {
                GetDataPassing += value;
            }
            remove
            {
                GetDataPassing -= value;
            }
        }
        #endregion

        #region 页面
        // 消息中心
        public static readonly MessageCenterPage messageCenterPage = new();
        // 联系人
        public static readonly ContactPersonPage contactPersonPage = new();
        #endregion

        #region API
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern bool DeleteObject(IntPtr delPtr);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, out DevMode devMode);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr MonitorFromPoint(Point pt, MonitorOptions dwFlags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out] MONITORINFOEX info);
        [DllImport("Shcore.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetDpiForMonitor(IntPtr hmonitor, DpiType dpiType, out uint dpiX, out uint dpiY);
        #endregion

        /// <summary>
        /// 根据句柄删除资源
        /// </summary>
        /// <param name="intPtr">句柄</param>
        /// <returns></returns>
        public static bool DeleteIntPtr(IntPtr delPtr)
        {
            return DeleteObject(delPtr);
        }

        /// <summary>
        /// 获取显示器列表
        /// </summary>
        /// <param name="lpDevice">null</param>
        /// <param name="iDevNum">第几个</param>
        /// <param name="lpDisplayDevice">返回数据</param>
        /// <param name="dwFlags">0</param>
        /// <returns></returns>
        public static bool GetDisplayDevices(string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags)
        {
            return EnumDisplayDevices(lpDevice, iDevNum, ref lpDisplayDevice, dwFlags);
        }

        /// <summary>
        /// 获取显示器配置
        /// </summary>
        /// <param name="deviceName">显示器名称</param>
        /// <param name="modeNum">模式</param>
        /// <param name="devMode">返回数据</param>
        /// <returns></returns>
        public static bool GetDisplaySettings(string deviceName, int modeNum, out DevMode devMode)
        {
            return EnumDisplaySettings(deviceName, modeNum, out devMode);
        }

        /// <summary>
        /// 根据坐标获取所在窗体句柄
        /// </summary>
        /// <param name="pt">坐标</param>
        /// <param name="dwFlags">设置</param>
        /// <returns></returns>
        public static IntPtr GetDisplayIntPtr(Point pt, MonitorOptions dwFlags)
        {
            return MonitorFromPoint(pt, dwFlags);
        }

        /// <summary>
        /// 获取窗体信息
        /// </summary>
        /// <param name="hmonitor">窗体句柄</param>
        /// <param name="info">返回数据</param>
        /// <returns></returns>
        public static bool GetDisplayInfo(IntPtr hmonitor, [In, Out] MONITORINFOEX info)
        {
            return GetMonitorInfo(hmonitor, info);
        }

        /// <summary>
        /// 获取屏幕Dpi
        /// </summary>
        /// <param name="hmonitor">窗体句柄</param>
        /// <param name="dpiType">Dpi类型</param>
        /// <param name="dpiX">x轴Dpi</param>
        /// <param name="dpiY">y轴Dpi</param>
        public static void GetDisplayDpi(IntPtr hmonitor, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            GetDpiForMonitor(hmonitor, dpiType, out dpiX, out dpiY);
        }

        /// <summary>
        /// 窗体消息通知
        /// </summary>
        /// <param name="window">显示窗体</param>
        /// <param name="messageType">0 成功 1 警告 2 默认 3 错误</param>
        /// <param name="message">提示信息</param>
        public static void MessageAlert(Type window, int messageType, string message)
        {
            if (window != null)
            {
                foreach (Delegate item in (GetMessage?.GetInvocationList()).Where(item => item.Target.GetType() == window))
                {
                    item.DynamicInvoke(messageType, message);
                }
            }
            else
            {
                GetMessage?.Invoke(messageType, message);
            }
        }

        /// <summary>
        /// 系统通知
        /// </summary>
        /// <param name="window">显示窗体</param>
        /// <param name="title">标题</param>
        /// <param name="message">消息</param>
        public static void NotificationAlert(Type window, string title, string message)
        {
            if (window != null)
            {
                foreach (Delegate item in (GetNotification?.GetInvocationList()).Where(item => item.Target.GetType() == window))
                {
                    item.DynamicInvoke(title, message);
                }
            }
            else
            {
                GetNotification?.Invoke(title, message);
            }
        }

        /// <summary>
        /// 显示蒙版
        /// </summary>
        /// <param name="show">显示隐藏</param>
        public static void ShowMask(bool show, bool loading = true)
        {
            GetAccording?.Invoke(show, loading);
        }

        /// <summary>
        /// 查找资源
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static T FindResource<T>(string key)
        {
            return (T)Application.Current?.Resources[key];
        }

        /// <summary>
        /// 切换路由
        /// </summary>
        /// <param name="routeName">页面名称</param>
        public static void SwitchRoute(PageType pageName)
        {
            GetRouted?.Invoke(pageName);
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="type">异常类</param>
        /// <param name="exception">异常信息</param>
        public static void RecordException(Type type, Exception exception)
        {
            string method = string.Empty;
            string location = string.Empty;
            if (exception.TargetSite != null)
            {
                method = exception.TargetSite.Name;
            }
            if (exception.StackTrace != null)
            {
                location = exception.StackTrace.ToString();
            }
            MessageAlert(null, 3, exception.Message);
            LogHelper.WriteErrorLog(type, method, location, exception.Message);
        }

        /// <summary>
        /// 类数据传递
        /// </summary>
        /// <param name="type">类</param>
        /// <param name="data">数据</param>
        public static void TransferringData(Type type, object data)
        {
            foreach (Delegate item in (GetDataPassing?.GetInvocationList()).Where(item => item.Target.GetType() == type))
            {
                item.DynamicInvoke(data);
            }
        }

        /// <summary>
        /// 请求服务器
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="mode">请求方式</param>
        /// <param name="responseObj">响应数据</param>
        /// <param name="requestObj">请求数据</param>
        /// <returns></returns>
        public static bool ServerRequest(string url, string mode, out JObject responseObj, JObject requestObj = null)
        {
            string returnStr = string.Empty;
            responseObj = null;
            try
            {
                switch (mode)
                {
                    case "GET": returnStr = HttpHelper.SendGet(url, true); break;
                    case "POST": returnStr = HttpHelper.SendPost(url, requestObj, true); break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(returnStr))
                {
                    MessageAlert(null, 3, FindResource<string>("ServerConnectionFailed"));
                    return false;
                }
                responseObj = JObject.Parse(returnStr);
                if (Convert.ToBoolean(responseObj["State"], cultureInfo))
                {
                    return true;
                }
                else
                {
                    MessageAlert(null, 3, responseObj["Message"].ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageAlert(null, 3, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="dataContent">请求数据</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static bool ServerUpload(string url, MultipartFormDataContent dataContent, out string fileName)
        {
            fileName = string.Empty;
            try
            {
                string returnStr = HttpHelper.SendUpload(url, dataContent, true);
                if (string.IsNullOrEmpty(returnStr))
                {
                    MessageAlert(null, 3, FindResource<string>("ServerConnectionFailed"));
                    return false;
                }
                JObject responseObj = JObject.Parse(returnStr);
                if (Convert.ToBoolean(responseObj["State"], cultureInfo))
                {
                    fileName = responseObj["Data"].ToString();
                    return true;
                }
                else
                {
                    MessageAlert(null, 3, responseObj["Message"].ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageAlert(null, 3, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeStamp(DateTime dateTime)
        {
            return ((dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString(cultureInfo);
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="aesKey">密钥</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string aesKey)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey) && aesKey.Length == 16)
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
                RijndaelManaged rm = new()
                {
                    Key = Encoding.UTF8.GetBytes(aesKey),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                data = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            return data;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">密文</param>
        /// <param name="aesKey">密钥</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string aesKey)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey) && aesKey.Length == 16)
            {
                byte[] toEncryptArray = Convert.FromBase64String(str);
                RijndaelManaged rm = new()
                {
                    Key = Encoding.UTF8.GetBytes(aesKey),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = rm.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                data = Encoding.UTF8.GetString(resultArray);
            }
            return data;
        }

        ///<summary>
        /// 随机字符串 
        ///</summary>
        /// <param name="length">长度</param>
        ///<returns></returns>
        public static string GetRandomString(int length)
        {
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            Random random = new(BitConverter.ToInt32(b, 0));
            string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string returnStr = string.Empty;
            for (int i = 0; i < length; i++)
            {
                returnStr += str.Substring(random.Next(0, str.Length - 1), 1);
            }
            return returnStr;
        }

        /// <summary>
        /// SHA256转换
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string GenerateSHA256(string str)
        {
            using SHA256 sHA256 = SHA256.Create();
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            //开始加密
            byte[] newBuffer = sHA256.ComputeHash(buffer);
            StringBuilder sb = new();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("x2", cultureInfo));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 消息盒子
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="messageBoxType">消息类型</param>
        /// <param name="message">消息</param>
        /// <param name="leftButton">左侧按钮(可空)</param>
        /// <param name="rightButton">右侧按钮(可空)</param>
        /// <returns></returns>
        public static MessageBoxCloseType AlertMessageBox(Window window, MessageBoxType messageBoxType, string message, MessageBoxButtonModel leftButton = null, MessageBoxButtonModel rightButton = null)
        {
            if (leftButton == null)
            {
                leftButton = new()
                {
                    Hint = FindResource<string>("Cancel")
                };
            }
            else if (string.IsNullOrEmpty(leftButton.Hint))
            {
                leftButton.Hint = FindResource<string>("Cancel");
            }
            if (rightButton == null)
            {
                rightButton = new()
                {
                    Hint = FindResource<string>("Confirm")
                };
            }
            else if (string.IsNullOrEmpty(rightButton.Hint))
            {
                rightButton.Hint = FindResource<string>("Confirm");
            }
            MessageBoxCloseType messageBoxCloseType = MessageBoxCloseType.Close;
            Dispatcher.Invoke(delegate
            {
                ClientMessageBox messageBox = new(messageBoxType, message, leftButton, rightButton)
                {
                    Owner = window.IsActive ? window : null
                };
                messageBox.ShowDialog();
                messageBoxCloseType = messageBox.CloseType;
            });
            return messageBoxCloseType;
        }

        /// <summary>
        /// 获取所有窗体信息
        /// </summary>
        /// <returns></returns>
        public static List<DisplayInfoModel> GetDisplayInfos()
        {
            List<DisplayInfoModel> displays = new();
            DisplayDevice d = new();
            d.CbSize = Marshal.SizeOf(d);
            for (uint id = 0; GetDisplayDevices(null, id, ref d, 0); id++)
            {
                if (d.DeviceState.HasFlag(DisplayDeviceState.AttachedToDesktop))
                {
                    if (GetDisplaySettings(d.DeviceName, currentSettings, out DevMode dEVMODE))
                    {
                        IntPtr intPtr = GetDisplayIntPtr(new Point(dEVMODE.DmPositionX, dEVMODE.DmPositionY), MonitorOptions.MONITORDEFAULTTONEAREST);
                        GetDpiForMonitor(intPtr, DpiType.Effective, out uint dpiX, out uint dpiY);
                        double zoomX = (double)dpiX / 96;
                        double zoomY = (double)dpiY / 96;
                        DisplayInfoModel displayInfo = new()
                        {
                            WindowIntPtr = intPtr,
                            DisplayWidth = dEVMODE.DmPelsWidth,
                            DisplayHeight = dEVMODE.DmPelsHeight,
                            DisplayLeft = dEVMODE.DmPositionX,
                            DisplayTop = dEVMODE.DmPositionY,
                            MainDisplay = d.DeviceState.HasFlag(DisplayDeviceState.PrimaryDevice),
                            ShowWidth = dEVMODE.DmPelsWidth / zoomX,
                            ShowHeight = dEVMODE.DmPelsHeight / zoomY,
                            ShowLeft = dEVMODE.DmPositionX / zoomX,
                            ShowTop = dEVMODE.DmPositionY / zoomY
                        };
                        displays.Add(displayInfo);
                    }
                }
                d.CbSize = Marshal.SizeOf(d);
            }
            return displays;
        }

        /// <summary>
        /// 获取缓存名称
        /// </summary>
        /// <param name="bytes">缓存内容</param>
        /// <returns></returns>
        public static string GetCacheFileName(byte[] bytes)
        {
            using SHA256 sHA256 = SHA256.Create();
            byte[] hash = sHA256.ComputeHash(bytes);
            return ToHex(hash);
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public static string ToHex(byte[] bytes)
        {
            return bytes.Aggregate(
                new StringBuilder(),
                (sb, b) => sb.Append(b.ToString("X2", cultureInfo)),
                sb => sb.ToString());
        }
    }
}
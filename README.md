# Dimension
# 基于 .NET 6 的在线音视频聊天项目
# WPF和ASP.NET API开发
# 使用第三方依赖介绍
**公用依赖**  
1. log4net 日志记录。
2. SignalR 用于服务器与客户端的通讯手段，该项目用于好友申请、消息提示、公告、聊天和音视频通话等一系列通知。
3. EntityFrameworkCore 操作数据库的ORM工具，服务端使用SqlServer，客户端使用Sqlite。
4. Newtonsoft.Json 序列化和反序列化JSON。

**服务端**  
1. TencentCloudSDK 操作腾讯云服务API，该项目用于管理通话房间。
2. aliyun-net-sdk-core 操作阿里云服务API，该项目用于短信服务。
3. CHSPinYinConv 获取中文拼音。
4. Portable.BouncyCastle TRTC加密使用。
5. SixLabors.ImageSharp 操作图片，因c#中操作图片需要微软的GDI绘图，但在linux上操作需要mono的libgdiplus库，处理效果并不理想。

**WPF端**  
1. TXLiteAV 操作腾讯云的TRTC服务，本地设备音视频推流、获取房间内其他用户音视频数据。
2. XamlAnimatedGif 播放GIF，因设备效率问题，改动作者源码后重新打包使用。
源库：https://github.com/XamlAnimatedGif/XamlAnimatedGif 问题：https://github.com/XamlAnimatedGif/XamlAnimatedGif/issues/160

**后台部署说明**  
1. 修改DimensionService.Common命名空间下 ClassHelper类  
![image](https://user-images.githubusercontent.com/84434846/159858628-cfcd7669-fae3-41b6-bd74-700e117c8870.png)  
请填写红框内付费服务内容，本程序使用阿里的短信服务和腾讯的TRTC服务，填写内容请见官方说明。  
2. 数据库  
该服务采用sqlserver2019数据库，并使用EF CORE作为主要的ORM框架，首次使用需要迁移数据库。  
打开程序包管理控制台，输入Update-Database InitialCreate  
![image](https://user-images.githubusercontent.com/84434846/159859559-e69a1d8e-fe6a-42f6-841e-980b20533ad4.png)  
该项目提供线上测试服务地址，http://47.96.133.119:5000  
# WPF端  
![image](https://user-images.githubusercontent.com/84434846/159860811-31419d6d-97a0-4f40-9536-c555d6140fd4.png)  
红框内容需与服务端保持一致  
已实现的功能  
1.登录|注册  
2.添加好友  
3.音视频在线通话  
4.聊天（图片、文字、富文本）  
5.截屏（多显示器不同dpi支持）  
# 界面展示
![image](https://user-images.githubusercontent.com/84434846/159876575-a9593d87-2b90-40b5-bd25-55f3d37518bc.png)  
![image](https://user-images.githubusercontent.com/84434846/159877738-b3197b00-5688-4a08-ab7f-52518da680ac.png)  
![image](https://user-images.githubusercontent.com/84434846/159876681-7482d993-3818-4863-90f9-c68725bb1449.png)  
![image](https://user-images.githubusercontent.com/84434846/159876750-6db8f92a-50b0-4a9e-b41e-53a2ce323cc6.png)  
![image](https://user-images.githubusercontent.com/84434846/159876843-9048b0a7-44bf-48ba-9e9b-a0839d02418e.png)  
![image](https://user-images.githubusercontent.com/84434846/159876933-8d07724e-100d-4c87-ab61-0db73fc47dbf.png)
![image](https://user-images.githubusercontent.com/84434846/159877246-c5b08b3a-86f8-4dac-a20e-8d9d6988dd45.png)
![image](https://user-images.githubusercontent.com/84434846/159877348-10285152-ae67-4e44-8fd9-9ea9e0cfddff.png)

# 功能演示
作者太懒，以后再写！  
我还是提供的测试账号和程序地址吧。  
不过需要安装NET6桌面运行时，这是下载地址：https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.3-windows-x64-installer  
# 测试用户  
1571221{1～9}177，
密码统一为12345678。  
所有用户登录信息我都放在程序包里啦，并且都添加了我做为好友。😄  
![image](https://user-images.githubusercontent.com/84434846/159868799-ed024f69-d91f-48d7-a52f-961502a89445.png)  
# 程序包
链接：https://pan.baidu.com/s/1aTh_710GpKIIHOHpvVCpBw?pwd=cp4o 
提取码：cp4o 
--来自百度网盘超级会员V4的分享
# 演示视频
链接：https://pan.baidu.com/s/1n-sQZFgO9GEhS80jHLVouA?pwd=85x3 
提取码：85x3 
--来自百度网盘超级会员V4的分享

# Dimension
基于 .NET 6 搭建的WPF程序和ASP.NET API服务。  
后台部署说明  
1.修改DimensionService.Common命名空间下 ClassHelper类  
![image](https://user-images.githubusercontent.com/84434846/159858628-cfcd7669-fae3-41b6-bd74-700e117c8870.png)  
请填写红框内付费服务内容，本程序使用阿里的短信服务和腾旭的TRTC服务，填写内容请见官方说明。  
如不填写将无法使用验证码登录和音视频童话。  
正确填写sqlserver的连接字符串。  
通话房间AppID需要和DimensionClient..Common命名空间下 ClassHelper类中callAppID常量保持一致。  
![image](https://user-images.githubusercontent.com/84434846/159858987-5bcbf1c1-0bd2-41be-b760-7fd3a8bab2e8.png)  
2.数据库  
该服务采用sqlserver2019数据库，并使用EF CORE作为主要的ORM框架，首次使用需要迁移数据库。  
打开程序包管理控制台，输入Update-Database InitialCreate  
![image](https://user-images.githubusercontent.com/84434846/159859559-e69a1d8e-fe6a-42f6-841e-980b20533ad4.png)  
该项目提供线上测试服务地址，http://47.96.133.119:5000  
WPF端  

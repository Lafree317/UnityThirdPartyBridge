 public enum CommandType
{
    // 设备信息
    SystemInfo = 1000000, // 获取设备信息
    RequestAccess = 1000001, // 请求设备权限
    ScreenOrientation = 1000002, // 屏幕方向

    // 登录
    Login = 1001000, // 登录
    Logout = 1001001, // 退出登录
    ModuleSwitch = 1001002, // 提审模块开关

    // H5
    WebAndUnity = 1002000, // H5和Unity互调

    // 分享
    Share = 1003000, // 分享
    PostingStyleBook = 1003001, // 合照分享

    // 支付
    Pay = 1004000, // 支付

    // 打点
    EventTrack = 1005000, // EventTrack
    Mission = 1005001, // 任务
    EventTrackBackEnd = 1005002, // 后端打点

    // 菜单栏
    NativeNavbarHandle = 1007000, // Native TabBar控制
    NavbarEvent = 1007001, // Native TabBar事件
    NavbarEnable = 1007002, // Native TabBar是否可点击

    // 路由
    BackToNative = 1009000, // Unity页面回到Native页面  已弃用
    GoToUnity = 1009001, // 跳转Unity页面

    // Network
    BannedUser = 1010000, // 封禁用户 当游戏网络请求返回200007时，唤起弹窗
    ErrorLog = 1010001, // 错误日志

    //User Info
    ProfileImg = 1011001, // 用户更换头像
    Username = 1011002, // 用户更换昵称

    //Audio
    PlayEffect = 1012000, // 播放音效
    PlayMusic = 1012001, // 播放音乐
    EffectSwitch = 1012002, // 音效开关
    MusicSwitch = 1012003, // 音乐开关

    //Screen Capture
    RunWayScreenCapture = 1013000, // 走秀录制视频

    // 弹窗
    RewardPopup = 1014000, // 通用奖励弹窗
    LevelPopup = 1014001, //升级弹窗
    ClosedActivePopup = 1014002, //unity活动弹窗全部关闭之后通知Native

    // 运行环境
    Environment = 1999000, // 切换运行环境
}
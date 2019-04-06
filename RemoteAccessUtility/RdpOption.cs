using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteAccessUtility
{
    public class RdpOption : RdpOptionBase
    {
        public GeneralOption General;
        public DisplayOption Display;
        public LocalResourceOption LocalResource;
        public ExperienceOption Experience;
        public DetailOption Detail;

        public RdpOption()
        {
            base._options = new Dictionary<string, object>();

            General = new GeneralOption(_options);
            Display = new DisplayOption(_options);
            LocalResource = new LocalResourceOption(_options);
            Experience = new ExperienceOption(_options);
            Detail = new DetailOption(_options);
        }

        public void Write(string filename)
        {
            using (var sw = new StreamWriter(filename))
            {
                foreach (var option in Options)
                {
                    sw.WriteLineAsync(option).Wait();
                }
            }
        }
    }

    public class RdpOptionBase
    {
        protected Dictionary<string, object> _options;

        protected List<string> Options => _options.Select(op => op.Key + op.Value).ToList();

        protected void Set(string key, int value)
        {
            if (_options.ContainsKey(key + ":i:"))
                _options.Remove(key + ":i:");
            _options.Add(key + ":i:", value);
        }

        protected void Get(string key, out int value)
        {
            var val = _options[key + ":i:"];
            value = (int)val;
        }

        protected void Set(string key, string value)
        {
            if (_options.ContainsKey(key + ":s:"))
                _options.Remove(key + ":s:");
            _options.Add(key + ":s:", value);
        }

        protected void Get(string key, out string value)
        {
            var val = _options[key + ":s:"];
            value = (string)val;
        }

        protected void Set(string key, byte[] value)
        {
            var val = BitConverter.ToString(value).Replace("-", "");
            if (_options.ContainsKey(key + ":b:"))
                _options.Remove(key + ":b:");
            _options.Add(key + ":b:", val);
        }

        protected void Get(string key, out byte[] value)
        {
            var val = (string)(_options[key + ":b:"]);
            var bs = new List<byte>();
            for (int i = 0; i < val.Length - 1; i += 2)
            {
                bs.Add(Convert.ToByte(val.Substring(i, 2), 16));
            }
            value = bs.ToArray();
        }
    }

    public class GeneralOption : RdpOptionBase
    {
        public GeneralOption(Dictionary<string, object> options)
        {
            base._options = options;

            ServerPort = 0xD3D;
        }

        /// <summary>
        /// コンピューター名またはIPアドレス
        /// </summary>
        public string FullAddress
        {
            get
            {
                Get("full address", out string val);
                return val;
            }
            set => Set("full address", value);
        }

        /// <summary>
        /// TCPポート
        /// </summary>
        public int ServerPort
        {
            get
            {
                Get("server port", out int val);
                return val;
            }
            set => Set("server port", value);
        }

        /// <summary>
        /// ユーザーアカウント
        /// </summary>
        public string Username
        {
            get
            {
                Get("username", out string val);
                return val;
            }
            set => Set("username", value);
        }

        public string Password
        {
            get
            {
                Get("password 51", out byte[] val);
                return DpApiAccessor.Decrypt(val);
            }
            set
            {
                var password = DpApiAccessor.Encrypt(DpApiAccessor.KeyType.MachineKey, value, string.Empty, "psw"); 
                Set("password 51", password);
            }
        }
    }

    public class DisplayOption : RdpOptionBase
    {
        public DisplayOption(Dictionary<string, object> options)
        {
            base._options = options;

            ScreenModeID = ScreenMode.Fullscreen;
            UseMultimon = false;
            SessionBpp = Bpp.TrueColor;
            DisplayConnectionBar = true;
            SmartSizing = false;
        }

        public enum ScreenMode
        {
            /// <summary>
            /// ウィンドウモード
            /// </summary>
            Window = 1,

            /// <summary>
            /// 全画面モード
            /// </summary>
            Fullscreen = 2
        }

        /// <summary>
        /// 画面モード
        /// </summary>
        public ScreenMode ScreenModeID
        {
            get
            {
                Get("screen mode id", out int val);
                return (ScreenMode)val;
            }
            set => Set("screen mode id", (int)value);
        }

        /// <summary>
        /// 画面幅
        /// </summary>
        public int Desktopwidth
        {
            get
            {
                Get("desktopwidth", out int val);
                return val;
            }
            set => Set("desktopwidth", value);
        }

        /// <summary>
        /// 画面高さ
        /// </summary>
        public int DesktopHeight
        {
            get
            {
                Get("desktopheight", out int val);
                return val;
            }
            set => Set("desktopheight", value);
        }

        /// <summary>
        /// マルチモニタの使用
        /// </summary>
        public bool UseMultimon
        {
            get
            {
                Get("use multimon", out int val);
                return val == 1;
            }
            set => Set("use multimon", value ? 1 : 0);
        }

        public enum Bpp
        {
            HighColor15Bit = 15,
            HighColor16Bit = 16,
            TrueColor = 24,
            HighestQuality = 32
        }

        /// <summary>
        /// 画面の色
        /// </summary>
        public Bpp SessionBpp
        {
            get
            {
                Get("session bpp", out int val);
                return (Bpp)val;
            }
            set => Set("session bpp", (int)value);
        }

        /// <summary>
        /// 接続バーの表示
        /// </summary>
        public bool DisplayConnectionBar
        {
            get
            {
                Get("displayconnectionbar", out int val);
                return val == 1;
            }
            set => Set("displayconnectionbar", value ? 1 : 0);
        }

        /// <summary>
        /// スマートサイジングの使用
        /// </summary>
        public bool SmartSizing
        {
            get
            {
                Get("smart sizing", out int val);
                return val == 1;
            }
            set => Set("smart sizing", value ? 1 : 0);
        }
    }

    public class LocalResourceOption : RdpOptionBase
    {
        public LocalResourceOption(Dictionary<string, object> options)
        {
            base._options = options;

            AudioPlayMode = AudioPlayModeConst.Local;
            AudioCaptureMode = AudioCaptureConst.NoCapture;
            KeyboardHook = KeyboardHookMode.FullScreen;
            RedirectClipboard = true;
        }

        public enum AudioPlayModeConst
        {
            /// <summary>
            /// ローカルコンピューターで再生
            /// </summary>
            Local = 0,

            /// <summary>
            /// リモートコンピューターで再生
            /// </summary>
            Remote = 1,

            /// <summary>
            /// 再生しない
            /// </summary>
            NoPlay = 2,
        }

        /// <summary>
        /// リモートオーディオ再生
        /// </summary>
        public AudioPlayModeConst AudioPlayMode
        {
            get
            {
                Get("audiomode", out int val);
                return (AudioPlayModeConst)val;
            }
            set => Set("audiomode", (int)value);
        }

        public enum AudioCaptureConst
        {
            /// <summary>
            /// 録音しない
            /// </summary>
            NoCapture = 0,

            /// <summary>
            /// ローカルコンピューターから録音
            /// </summary>
            Local = 1,
        }

        /// <summary>
        /// リモートオーディオ録音
        /// </summary>
        public AudioCaptureConst AudioCaptureMode
        {
            get
            {
                Get("audiocapturemode", out int val);
                return (AudioCaptureConst)val;
            }
            set => Set("audiocapturemode", (int)value);
        }

        public enum KeyboardHookMode
        {
            /// <summary>
            /// ローカルコンピューター
            /// </summary>
            Local = 0,

            /// <summary>
            /// リモートコンピューター
            /// </summary>
            Remote = 1,

            /// <summary>
            /// 全画面
            /// </summary>
            FullScreen = 2,
        }

        /// <summary>
        /// Windowsキーの組み合わせ
        /// </summary>
        public KeyboardHookMode KeyboardHook
        {
            get
            {
                Get("keyboardhook", out int val);
                return (KeyboardHookMode)val;
            }
            set => Set("keyboardhook", (int)value);
        }

        /// <summary>
        /// クリップボードリダイレクト
        /// </summary>
        public bool RedirectClipboard
        {
            get
            {
                Get("redirectclipboard", out int val);
                return val == 1;
            }
            set => Set("redirectclipboard", value ? 1 : 0);
        }

        /// <summary>
        /// プリンタリダイレクト
        /// </summary>
        public bool RedirectPrinters
        {
            get
            {
                Get("redirectprinters", out int val);
                return val == 1;
            }
            set => Set("redirectprinters", value ? 1 : 0);
        }
    }

    public class ExperienceOption : RdpOptionBase
    {
        public ExperienceOption(Dictionary<string, object> options)
        {
            base._options = options;

            ConnectionType = ConnectionTypeConst.LowSpeed;
            BitmapCachePersistEnable = true;
            AutoReconnectionEnabled = true;
        }


        public enum ConnectionTypeConst
        {
            /// <summary>
            /// モデム (56Kbps)
            /// </summary>
            Modem = 1,

            /// <summary>
            /// 低速ブロードバンド (256Kbps - 2Mbps)
            /// </summary>
            LowSpeed = 2,

            /// <summary>
            /// 衛星 (2Mbps - 16Mbps)
            /// </summary>
            Satellite = 3,

            /// <summary>
            /// 高速ブロードバンド (2Mbps - 10Mbps)
            /// </summary>
            HighSpeed = 4,

            /// <summary>
            /// WAN (10Mbps - )
            /// </summary>
            Wan = 5,

            /// <summary>
            /// LAN (10Mbps - )
            /// </summary>
            Lan = 6,
        }

        /// <summary>
        /// 接続品質
        /// </summary>
        public ConnectionTypeConst ConnectionType
        {
            get
            {
                Get("connection type", out int val);
                return (ConnectionTypeConst)val;
            }
            set => Set("connection type", (int)value);
        }

        /// <summary>
        /// 壁紙の無効化
        /// </summary>
        public bool DisableWallpaper
        {
            get
            {
                Get("disable wallpaper", out int val);
                return val == 1;
            }
            set => Set("disable wallpaper", value ? 1 : 0);
        }

        /// <summary>
        /// フォントスムージングの許可
        /// </summary>
        public bool AllowFontSmoothing
        {
            get
            {
                Get("allow font smoothing", out int val);
                return val == 1;
            }
            set => Set("allow font smoothing", value ? 1 : 0);
        }

        /// <summary>
        /// デスクトップコンポジションの許可
        /// </summary>
        public bool AllowDesktopComposition
        {
            get
            {
                Get("allow desktop composition", out int val);
                return val == 1;
            }
            set => Set("allow desktop composition", value ? 1 : 0);
        }

        /// <summary>
        /// ドラッグウィンドウ表示の無効化
        /// </summary>
        public bool DisableFullWindowDrag
        {
            get
            {
                Get("disable full window drag", out int val);
                return val == 1;
            }
            set => Set("disable full window drag", value ? 1 : 0);
        }

        /// <summary>
        /// メニューウィンドウアニメーションの無効化
        /// </summary>
        public bool DisableMenuAnims
        {
            get
            {
                Get("disable menu anims", out int val);
                return val == 1;
            }
            set => Set("disable menu anims", value ? 1 : 0);
        }

        /// <summary>
        /// 資格スタイルの無効化
        /// </summary>
        public bool DisableThemes
        {
            get
            {
                Get("disable themes", out int val);
                return val == 1;
            }
            set => Set("disable themes", value ? 1 : 0);
        }

        /// <summary>
        /// ビットマップキャッシュの保持
        /// </summary>
        public bool BitmapCachePersistEnable
        {
            get
            {
                Get("bitmapcachepersistenable", out int val);
                return val == 1;
            }
            set => Set("bitmapcachepersistenable", value ? 1 : 0);
        }

        /// <summary>
        /// 自動再接続の有効化
        /// </summary>
        public bool AutoReconnectionEnabled
        {
            get
            {
                Get("autoreconnection enabled", out int val);
                return val == 1;
            }
            set => Set("autoreconnection enabled", value ? 1 : 0);
        }
    }

    public class DetailOption : RdpOptionBase
    {
        public DetailOption(Dictionary<string, object> options)
        {
            base._options = options;

            AuthenticationLevel = AuthenticationLevelConst.NoAuthentication;
        }

        public enum AuthenticationLevelConst
        {
            /// <summary>
            /// 接続し、警告メッセージは表示しない
            /// </summary>
            ConnectNoWarn = 0,

            /// <summary>
            /// 接続しない
            /// </summary>
            NoConnect = 1,

            /// <summary>
            /// 警告メッセージを表示する
            /// </summary>
            Warn = 2,

            /// <summary>
            /// 認証要求をしない
            /// </summary>
            NoAuthentication = 3
        }

        /// <summary>
        /// サーバー認証
        /// </summary>
        public AuthenticationLevelConst AuthenticationLevel
        {
            get
            {
                Get("authentication level", out int val);
                return (AuthenticationLevelConst)val;
            }
            set => Set("authentication level", (int)value);
        }
    }
}

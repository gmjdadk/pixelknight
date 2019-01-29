using System;
using System.Collections.Generic;

namespace PixelKnight.Models
{
    public class Configuration
    {
        private bool _useMultiThreading = true;
        public static bool sceneTransitioning;
        public static bool isChinaBuild;
        public static bool mobileUITestMode;
        public static bool firebaseAvailable;
        public static bool enableLogging;
        private bool isDemoMode;

        public bool BattleOptimizerEnabled
        {
            get
            {
                return PlayerPrefs.GetInt(nameof(BattleOptimizerEnabled), 1) == 0;
            }
        }

        public bool TestMode
        {
            get
            {
                return this._testMode;
            }
            set
            {
                this._testMode = value;
            }
        }

        public bool StartTutorialFromBeginning
        {
            get
            {
                return this._startTutorialFromBeginning;
            }
        }

        public int MaxNumberOfCharactersOwned
        {
            get
            {
                return 120;
            }
        }

        public string ChecksumKey
        {
            get
            {
                return "5343";
            }
        }

        public bool ShouldHideEnglish
        {
            get
            {
                return Configuration.isChinaBuild;
            }
        }

        public bool ShouldHideChat
        {
            get
            {
                return Configuration.isChinaBuild;
            }
        }

        public bool ShouldHideBank
        {
            get
            {
                return false;
            }
        }

        public bool ShouldUseMultiThreading
        {
            get
            {
                if (this._useMultiThreading)
                    return !this.IsUsingWebPlayer;
                return false;
            }
        }

        public bool ShouldShowAds
        {
            get
            {
                if (!Configuration.isChinaBuild)
                    return PlayerPrefs.GetInt("ShowAds", 1) == 1;
                return false;
            }
        }

        public DateTime OriginDate
        {
            get
            {
                return new DateTime(2016, 1, 6);
            }
        }

        public bool IsUsingWebPlayer
        {
            get
            {
                return Application.get_platform() == 17;
            }
        }

        public string ServerUrl
        {
            get
            {
                string str = "http://apibackup.pixelstarships.com";
                string server = this.Server;
                if (server != null)
                {
                    if (Configuration.\u003C\u003Ef__switch\u0024map0 == null)
            Configuration.\u003C\u003Ef__switch\u0024map0 = new Dictionary<string, int>(9)
            {
              {
                "Dev",
                0
              },
              {
                "Xin",
                1
              },
              {
                "Prod1",
                2
              },
              {
                "Prod2",
                3
              },
              {
                "Staging",
                4
              },
              {
                "Backup",
                5
              },
              {
                "Test",
                6
              },
              {
                "China",
                7
              },
              {
                "China2",
                8
              }
            };
                    int num;
                    if (Configuration.\u003C\u003Ef__switch\u0024map0.TryGetValue(server, out num))
          {
                        switch (num)
                        {
                            case 0:
                                str = "http://apidev.pixelstarships.com";
                                break;
                            case 1:
                                str = "http://172.20.10.4";
                                break;
                            case 2:
                                str = "https://api.pixelstarships.com";
                                break;
                            case 3:
                                str = "https://api2.pixelstarships.com";
                                break;
                            case 4:
                                str = "http://apistaging.pixelstarships.com";
                                break;
                            case 5:
                                str = "http://apibackup.pixelstarships.com";
                                break;
                            case 6:
                                str = "http://apitest.pixelstarships.com";
                                break;
                            case 7:
                                str = "http://apicn.pixelstarships.com";
                                break;
                            case 8:
                                str = "http://139.199.82.223";
                                break;
                        }
                    }
                }
                if (this.IsUsingWebPlayer)
                    return str.Replace("http://", "https://");
                return str;
            }
        }

        public string Server
        {
            get
            {
                if (Application.get_isEditor())
                    return PlayerPrefs.GetString("EditorServer", "Dev");
                if (Configuration.isChinaBuild)
                    return "Backup";
                return Debug.isDebugBuild ? "Dev" : "Prod1";
            }
        }

        public string ServerName
        {
            get
            {
                if (this.Server == "Prod1" || this.Server == "Prod2")
                    return "Prod";
                return this.Server;
            }
        }

        public string SecureServerUrl
        {
            get
            {
                if (this.ServerName == "Prod" || this.ServerName == "Staging")
                    return this.ServerUrl.Replace("http://", "https://");
                return this.ServerUrl;
            }
        }

        public string DeviceStoragePath => string.Empty;

        public string PusherAPIKey
        {
            get
            {
                return this.Server == "Dev" || this.Server == "China" || SingletonManager<SystemManager>.Instance.settings.IsDebug ? "ae31b391bb01fb1dbf13" : "258a49f843d21115d7f7";
            }
        }

        public string PublicChatChannel => "public-en";
    }
}

using Native.Csharp.App.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Main
{
    class Repeater
    {
        private static Random ran = new Random();
        private int cooldown;
        private string content;
        private string delayContent;
        private int delayCountdown;
        private bool repeated; //已经复读过本句

        public Repeater(string msg)
        {
            content = msg;
            delayContent = "";
            cooldown = 0;
            repeated = false;
        }


        static public bool CheckIfIgnore(string msg)
        {
            if (msg.Length == 0 || msg[0] == '.') //对指令不复读
                return true;
            return false;
        }
        public bool RandomlyDecide(string msg)
        {
            if (msg == content)
                return ran.Next() % 10 < 9;
            else
                return ran.Next() % 300 == 0;
        }

        public void DoRepeat(GroupMessageEventArgs e)
        {
            string msg = e.Msg;

            int mode = RollMode(msg);
            switch (mode)
            {
                case 0: //风怒复读机
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, msg);
                        Common.CqApi.SendGroupMessage(e.FromGroup, msg);
                        while (ran.Next() % 10 == 0)
                            Common.CqApi.SendGroupMessage(e.FromGroup, msg);
                    }
                    break;

                case 1: //劣质复读机
                    {
                        List<string> chaostring = Cutstring(msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        int len = ran.Next() % Math.Max(1, chaostring.Count - 1) + 1;
                        for (int i = 0; i < len; i++)
                            stringBuilder.Append(chaostring[i]);
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 2: //复读机模拟器
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, msg + " +1");
                    }
                    break;

                case 3: //雕版印刷复读机
                    {
                        string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"data\image\fdj\";
                        FileInfo[] files = new DirectoryInfo(filePath).GetFiles();
                        string randomPicName = files[ran.Next() % files.Length].Name.Replace(".cqimg", "");
                        string result = Common.CqApi.CqCode_Image(@"fdj\" + randomPicName);
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 4: //帕金森复读机
                    {
                        string result = msg + msg;
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 5: //倒装复读机
                    {
                        List<string> chaostring = Cutstring(msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = chaostring.Count - 1; i >= 0; i--)
                            stringBuilder.Append(chaostring[i]);
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 6: //延迟复读机
                    {
                        if (delayContent != "") //有正在准备延迟复读的语句
                        {//强制提前复读
                            Common.CqApi.SendGroupMessage(e.FromGroup, delayContent);
                            delayContent = "";
                        }
                        delayContent = msg;
                        delayCountdown = ran.Next() % 10 + 5;
                    }
                    break;

                case 7: //沙雕复读机
                    {
                        StringBuilder stringBuilder = new StringBuilder(msg);
                        int n = ran.Next() % 9 + 1;
                        for (int i = 0; i < n; i++)
                            stringBuilder.Append((char)(ran.Next() % 26 + 'a'));
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 8: //野兽复读机
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, msg + "（ 便 乘 ）");
                    }
                    break;

                case 9: //高冷复读机
                    {
                        List<string> replys = new List<string> {
                            "哦",
                            "哦，知道了",
                            "哦，知道了（敷衍）"
                        };
                        Common.CqApi.SendGroupMessage(e.FromGroup, replys[ran.Next() % replys.Count]);
                    }
                    break;

                case 10: //混乱复读机
                    {
                        List<string> chaostring = Cutstring(msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        while (chaostring.Count > 0)
                        {
                            int i = ran.Next() % chaostring.Count;
                            stringBuilder.Append(chaostring[i]);
                            chaostring.RemoveAt(i);
                        }
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 11: //混乱复读机·改
                    {
                        List<string> chaostring = Cutstring(msg);
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < chaostring.Count; i++)
                            stringBuilder.Append(chaostring[ran.Next() % chaostring.Count]);
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 12: //回声复读机
                    {
                        List<string> chaostring = Cutstring(msg);
                        StringBuilder stringBuilder = new StringBuilder(msg);
                        if (chaostring.Count >= 2)
                        {
                            int n = ran.Next() % 9 + 1;
                            for (int i = 0; i < n; i++)
                                stringBuilder.Append(chaostring.Last());
                        }
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;

                case 13: //大喘气复读机
                    {
                        List<string> chaostring = Cutstring(msg);
                        StringBuilder stringBuilder = new StringBuilder(chaostring[0]);
                        for (int i = 1; i < chaostring.Count; i++)
                            stringBuilder.Append(" " + chaostring[i]);
                        string result = stringBuilder.ToString();
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;
                case 14: //火星复读机
                    {
                        string result = Hxw.translate(msg);
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                    }
                    break;
                default:
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, msg);//普通复读
                    }
                    break;
            }
            repeated = true;
            cooldown = RollCoolDown();
        }


        public bool CDDec() //CDDec反馈当前是否已经结束CD，并且，如果没有结束CD，令CD-1。
        {
            if (cooldown <= 0)
                return true;
            else
            {
                cooldown--;
                return false;
            }
        }
        public void Update(string msg)
        {
            if (content != msg)
            {
                content = msg;
                repeated = false;
            }

        }
        public bool AlreadyRepeated()
        {
            return repeated;
        }
        public bool Delaying()
        {
            return delayContent != "";
        }
        public void DoDelayRepeat(GroupMessageEventArgs e)
        {
            if (delayCountdown > 0)
                delayCountdown--;

            if (delayContent != "" && delayCountdown <= 0)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, delayContent);
                delayContent = "";
            }
        }

        private List<string> Cutstring(string msg)
        {
            List<string> stringpart = new List<string>();
            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == '\r')   //忽略所有\r\n中的\r只保留\n，酷Q最后会自动将\n重新转义回\r\n。
                    continue;
                if (msg[i] == '[')
                {
                    Match cqMatch = Regex.Match(msg.Substring(i), @"(^\[CQ:.+?\])");
                    if (cqMatch.Success)
                    {
                        string cqcode = cqMatch.Groups[1].Value;
                        stringpart.Add(cqcode);
                        i += cqcode.Length - 1;
                        continue;
                    }
                }
                stringpart.Add(msg[i].ToString());
            }
            if (stringpart.Count == 0)
                throw new Exception("欲分解的字符串为空");
            return stringpart;
        }

        private int RollMode(string str)
        {
            int mode = ran.Next() % 20;
            if (mode == 3 && str != content) //如果是抽选复读，不触发雕版印刷复读机
                mode = 99;
            /*if (int.TryParse(str.Substring(0,2),out int temp))// 调试用
                return temp;*/
            return mode;
        }
        private int RollCoolDown()
        {
            return ran.Next() % 10 + 5;
        }
    }

}

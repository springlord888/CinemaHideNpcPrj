using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CinemaHideNpc
{
    public struct CinemaDataCommon
    {
        public static string RES_DEVELOP_PATH = "";//"E:\\sandBox\\sandbox_res\\resource\\develop\\";//动态调整
        public const string DEVELOP_STRING = "develop";
        public const string CinemaDataRootPath = "client/json/cinema";
        public const string CinemaListFilePath = CinemaDataRootPath + "/cinema_list.json";
        public const string CinemaQualityFilePath = CinemaDataRootPath + "/cinema_quality_list.json";
        public const string CinemaResRootPath = "prefab/cinema/animation/";

        public const string jsonExtName = ".json";
        public const string Aspect43Prefix = "_4x3";
        public const string FemalePrefix = "_f";

        public const string BridgeHideNpcCinemaJsonName = "bridge_hide_npc_cinema.json";

        public static string GetCinemaListFullPath()
        {
            return PathTool.Combine(RES_DEVELOP_PATH, CinemaListFilePath);
        }

        //cinemaName: "plot/anim_polt_gongzuotaiyanshi"
        public static string GetCinemaJsonFileFullPath(string cinemaName)
        {
            string str = PathTool.Combine(RES_DEVELOP_PATH, CinemaDataRootPath);
            str = PathTool.Combine(str, cinemaName);
            str = string.Concat(str, jsonExtName);
            return str;
        }
    }

    
    public enum CinemaType
    {
        None = 0,
        Live = 1,    // 实景动画（位置可变的实景动画）
        Anchor = 2,    // 定点动画（位置固定的实景动画）
        Camera = 3,    // 镜头动画（过场动画）
                       
    }
    public enum StaticCinemaTriggerType
    {
        Min = 0,
        EnterScene = 1,
        EnterFuncArea = 2,
        Gather = 11,
        Attacked = 21,
        Killed = 22,
        Max,
    }
}

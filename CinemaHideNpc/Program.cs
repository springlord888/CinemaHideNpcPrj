using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using LitJson;


namespace CinemaHideNpc
{
    
    class Program
    {
        static string path = @"E://sandBox//sandbox_res//resource//develop//cinema_hidenpc_config_tool//CinemaHideNpc//CinemaHideNpc//test_cinema.json";
        static string IMPORT_STRING_ARG = "1";
        static string EXPORT_STRING_ARG = "2";
        static void Main(string[] args)
        {
            bool isImport = true;
            if (null != args)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains(IMPORT_STRING_ARG))
                    {
                        isImport = true;
                        break;
                    }
                    else if(args[i].Contains(EXPORT_STRING_ARG))
                    {
                        isImport = false;
                        break;
                    }
                }
            }
            //TestSave();
            //TestLoad();

            
            // 读取动画本地配置数据
            LoadCinemaJsonTool loadCinemaJsonTool = new LoadCinemaJsonTool();
            if (isImport)
            {
                loadCinemaJsonTool.Import();
                Console.WriteLine("中间json文件生成完毕,可导入Excel......");
            }
            else
            {
                loadCinemaJsonTool.Export();
                Console.WriteLine("动画批量隐藏npc的json文件生成完毕......");
                Console.WriteLine("按下任意键关闭窗口......");
                Console.ReadKey();
            
            }

            
            
        }

       

        public static void TestSave()
        {
            //新建数据
            CameraCinemaJsonData cameraCinemaJsonData = new CameraCinemaJsonData();
            cameraCinemaJsonData.playMode = CinemaPlayMode.Default;
            cameraCinemaJsonData.extend = new CinemaExtendJsonData();
            CinemaExtendJsonData.Material m1 = new CinemaExtendJsonData.Material();
            m1.data = new CinemaExternDataMaterial();
            m1.data.targetPath = "player/create_login_actor";
            m1.data.colorList = new List<MatColor>() {
                new MatColor("_RimColor", 0.0f, 0.4f, 1.0f, 1.0f)
            };
            m1.data.floatList = new List<MatFloat>() {
                new MatFloat("_RimIntensity",5.0f),
                new MatFloat("_RimPower",2.7300000190734865f),
            };

            cameraCinemaJsonData.extend.materials = new CinemaExtendJsonData.Material[] { m1 };
            cameraCinemaJsonData.extend.operations = new CinemaExtendJsonData.Operation[0];
            cameraCinemaJsonData.extend.postEffectFirstIndexs = new int[] { 85, 86 };
            cameraCinemaJsonData.extend.renderLods = new int[] { -1, 1, 1, 1, 1 };


            cameraCinemaJsonData.needReplayForDelaying = true;
            cameraCinemaJsonData.allowCinemaGroup = true;
            cameraCinemaJsonData.groupMode = CinemaGroupMode.ShunXuBoFang;
            cameraCinemaJsonData.groupCinemas = new string[] { "aaa", "bbb", "ccc" };
            cameraCinemaJsonData.hideHostPlayer = true;
            cameraCinemaJsonData.hideOtherPlayer = true;
            cameraCinemaJsonData.hideNpcIds = new int[] { 111, 222, 333, 444, 555 };
            cameraCinemaJsonData.handleMainCamera = true;
            cameraCinemaJsonData.handleUICamera = true;
            cameraCinemaJsonData.canSkip = true;
            cameraCinemaJsonData.allowSelectScene = true;
            // 写入
            Persistence.json.Save(path, cameraCinemaJsonData);
        }

        public static void TestLoad()
        {
            CameraCinemaJsonData cameraCinemaJsonData = Persistence.json.LoadFile<CameraCinemaJsonData>(path);
        }

        
    }
}

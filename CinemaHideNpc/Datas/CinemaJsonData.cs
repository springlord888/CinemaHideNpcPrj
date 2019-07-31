using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHideNpc
{
    public interface ISerializationCallbackReceiver
    {
        void OnAfterDeserialize();
        void OnBeforeSerialize();
    }

    [Serializable]
    public class CinemaListJsonData : ISerializationCallbackReceiver
    {
        [Serializable]
        public class Data
        {
            public int id = 0;

            public CinemaType type = CinemaType.None;
            // 最简动画路径，掐头去尾只保留不同的部分，加载时在拼接成完整路径
            public string path = "";
            public string path_aspect43 = "";
            // 分男女的女性动画路径，以后可能要干掉
            public string path2 = "";
            // 排队优先级 0:不排队，1-n:数值越大越优先
            public int queuedPriority = 1;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

        public Data[] cinema_list = null;
    }



    [Serializable]
    public class CinemaJsonData : ISerializationCallbackReceiver
    {

        public static CinemaJsonData LoadData(CinemaType type, string dataPath)
        {
            CinemaJsonData result = null;
            switch (type)
            {
                case CinemaType.Live:
                    result = Persistence.json.LoadFile<CinemaJsonData>(dataPath);
                    break;
                case CinemaType.Anchor:
                    result = Persistence.json.LoadFile<AnchorCinemaJsonData>(dataPath);
                    break;
                case CinemaType.Camera:
                    result = Persistence.json.LoadFile<CameraCinemaJsonData>(dataPath);
                    break;
            }
            return result;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

        public CinemaPlayMode playMode = CinemaPlayMode.Default;
        public CinemaExtendJsonData extend;
        public bool needReplayForDelaying = false;

        public bool allowCinemaGroup;
        public CinemaGroupMode groupMode = CinemaGroupMode.NONE;
        public string[] groupCinemas = new string[0];

    }

    [Serializable]
    public class AnchorCinemaJsonData : CinemaJsonData
    {
        public Vector3 anchorPos = new Vector3(0,0,0);
        public Vector3 anchorDir = new Vector3(0, 0, 0);
        public Vector3 anchorScale = new Vector3(1, 1, 1);
        public string[] skipSavedTags = null;
    }
    [Serializable]
    public class CameraCinemaJsonData : CinemaJsonData
    {
        public bool hideHostPlayer = true;
        public bool hideOtherPlayer = true;
        public int[] hideNpcIds = null;

        public bool handleMainCamera = true;
        public bool handleUICamera = false;

        public bool canSkip = false;
        public bool allowSelectScene = false;
    }
    /******************************/
    public enum CinemaPlayMode
    {
        Default = 0,
        Mode1Self = 1,
        Mode2Target = 2,
        Mode3Self2Target = 3,
        Mode4Target2Self = 4,
        Mode5InteracBetween = 5,
        Mode6InteracSelf = 6,
        Mode7InteracTarget = 7
    }

    public enum CinemaGroupMode
    {
        NONE = 0,
        ShunXuBoFang = 1
    }


    public enum CinemaOperationType
    {
        Gather,
        Attack,
        Capture,
    }

    public enum CinemaExternDataType
    {
        None,
        UI,
        Operation,
        Material,
        Common,
        SelectScene,
        CinemaGroup,
    }


    [Serializable]
    public partial class CinemaExtendJsonData : ISerializationCallbackReceiver
    {
        public interface IExtendData { }
        [Serializable]
        public class Operation : IExtendData
        {
            public CinemaExternDataOperation data;
        }
        [Serializable]
        public class Material : IExtendData
        {
            public CinemaExternDataMaterial data;
        }


        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
        }

        public Operation[] operations = new Operation[0];
        public Material[] materials = new Material[0];
        public int[] postEffectFirstIndexs = new int[0];  // 记录首帧有后处理TrackNode索引，需要在播放的第一帧应用
        public int[] renderLods = new int[0];        // -1=关闭，0=VISIBLE_LOD_DEFAULT，>=VISIBLE_LOD_MAX=真实值
    }



    [Serializable]
    public class CinemaExternDataOperation 
    {
        public CinemaExternDataType Type { get { return CinemaExternDataType.Operation; } }
        public override string ToString()
        {
            return _optType.ToString();
        }


        public CinemaOperationType OptType { get { return (CinemaOperationType)_optType; } set { _optType = (int)value; } }

        public double startDelay = 0;//litJson不支持List<float>的object到string的转换，强行改成double
        public int startGotoFrame = -1;
        public string startGotoTag = "";
        public double endDelay = 0;//litJson不支持List<float>的object到string的转换，强行改成double
        public int endGotoFrame = -1;
        public string endGotoTag = "";

        private int _optType;
    }


   
    public interface ICinemaExternData
    {
        //public CinemaExternDataType Type { get; }
    }

    [Serializable]
    public class CinemaExternDataMaterial : ICinemaExternData
    {
        
        //public CinemaExternDataType Type { get { return CinemaExternDataType.Material; } }

    
        public string targetPath = "";
        public List<MatColor> colorList = new List<MatColor>();
        public List<MatFloat> floatList = new List<MatFloat>();
    }

    //litJson不支持List<float>的object到string的转换，强行改成 double

    [Serializable]
    public class Vector3
    {
        public Vector3() { }
        public Vector3(float a, float b, float c) { x = a;  y = b; z = c; }
        public double x = 0;
        public double y = 0;
        public double z = 0;

    }

    //litJson不支持List<float>的object到string的转换，强行改成double
    [Serializable]
    public class Color
    {
        public Color() { }
        public Color(float rr, float gg,float bb, float aa) {
            r = rr;
            g = gg;
            b = bb;
            a = aa;

        }
        public double r = 0.0f;
        public double g = 0.0f;
        public double b = 0.0f;
        public double a = 0.0f;

    }

    //litJson不支持List<float>的object到string的转换，强行改成double
    public class MatColor
    {
        public MatColor() { }
        public MatColor(string str,float rr, float gg, float bb, float aa)
        {
            name = str;
            value.r = rr;
            value.g = gg;
            value.b = bb;
            value.a = aa;
        }
        public string name = "";
        public Color value = new Color();
    }

    public class MatFloat
    {
        public MatFloat() { }
        public MatFloat(string str, float a)
        {
            name = str;
            value = a;
        }
        public string name = "";
        public double value = 0;
    }



    /*******中间json数据定义************/
    [Serializable]
    public class BridgeHideNpcCinemaJsonData
    {
        [Serializable]
        public class JsonData
        {
            public int npc_id = 0;
            public CinemaType cinema_type = CinemaType.None;
            public int[] cinema_ids = new int[0];
        }
        public BridgeHideNpcCinemaJsonData() { }

        public JsonData[] data = new JsonData[0];
    }
}

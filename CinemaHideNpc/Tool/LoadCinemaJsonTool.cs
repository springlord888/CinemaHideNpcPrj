using System;
using System.Collections.Generic;
using System.Text;
//读取本地动画配置
namespace CinemaHideNpc
{
    public class LoadCinemaJsonTool
    {
        public Dictionary<int, CinemaJsonData> cinemaIdMapCinemaJsonData = new Dictionary<int, CinemaJsonData>();
        public Dictionary<string, CinemaJsonData> cinemaNameMapCinemaJsonData = new Dictionary<string, CinemaJsonData>();
        public Dictionary<int, string> cinemaIdMapCinemaName = new Dictionary<int, string>();
        public Dictionary<CinemaType, List<int>> cinemaTypeMapCinemaIds = new Dictionary<CinemaType, List<int>>();
        public Dictionary<int, List<int>> npcIdMapCinemaIds = new Dictionary<int, List<int>>();
        public CinemaListJsonData cinemaListData = null;

        public LoadCinemaJsonTool() {
            //初始化develop的文件路径
            //E:\\sandBox\\sandbox_res\\resource\\develop\\cinema_hidenpc_config_tool\\CinemaHideNpc\\CinemaHideNpc\\bin\\Debug\\
            string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //查找develop
            int indexOfDevelop = BaseDirectory.IndexOf(CinemaDataCommon.DEVELOP_STRING);
            if (indexOfDevelop > -1)
            {
                // "E:\\sandBox\\sandbox_res\\resource\\develop\\"
                CinemaDataCommon.RES_DEVELOP_PATH = BaseDirectory.Substring(0, indexOfDevelop + 1 + CinemaDataCommon.DEVELOP_STRING.Length);
            }
        }

        //读取本地的动画相关json数据并缓存
        private void _LoadAndCacheCinemaJsonData()
        {
            cinemaIdMapCinemaJsonData.Clear();
            cinemaNameMapCinemaJsonData.Clear();
            cinemaIdMapCinemaName.Clear();
            cinemaTypeMapCinemaIds.Clear();
            npcIdMapCinemaIds.Clear();
            cinemaListData = null;
            //加载cinema_list数据        
            cinemaListData = Persistence.json.LoadFile<CinemaListJsonData>(CinemaDataCommon.GetCinemaListFullPath());
            int count = 0;
            if (null != cinemaListData)
            {
                count = cinemaListData.cinema_list.Length;
            }

            string cinemaJsonFullPath = "";
            for (int i = 0; i < count; i++)
            {
                var lData = cinemaListData.cinema_list[i];
                cinemaJsonFullPath = CinemaDataCommon.GetCinemaJsonFileFullPath(lData.path);
                CinemaJsonData cinemaJsonData = CinemaJsonData.LoadData(lData.type, cinemaJsonFullPath);

                //缓存数据1
                CinemaJsonData tmpCinemaJsonData = null;
                if (!cinemaIdMapCinemaJsonData.TryGetValue(lData.id, out tmpCinemaJsonData))
                {
                    cinemaIdMapCinemaJsonData.Add(lData.id, cinemaJsonData);
                }
                else
                {
                    cinemaIdMapCinemaJsonData[lData.id] = cinemaJsonData;
                }

                //缓存数据2
                CinemaJsonData tmpCinemaJsonData2 = null;
                if (!cinemaNameMapCinemaJsonData.TryGetValue(lData.path, out tmpCinemaJsonData2))
                {
                    cinemaNameMapCinemaJsonData.Add(lData.path, cinemaJsonData);
                }
                else
                {
                    cinemaNameMapCinemaJsonData[lData.path] = cinemaJsonData;
                }

                //缓存数据3
                string tempName = "";
                if (!cinemaIdMapCinemaName.TryGetValue(lData.id, out tempName))
                {
                    cinemaIdMapCinemaName.Add(lData.id, lData.path);
                }
                else
                {
                    cinemaIdMapCinemaName[lData.id] = lData.path;
                }
                //缓存数据4
                List<int> tempList;
                if (!cinemaTypeMapCinemaIds.TryGetValue(lData.type,out tempList))
                {
                    cinemaTypeMapCinemaIds[lData.type] = new List<int>();
                }
                if (!cinemaTypeMapCinemaIds[lData.type].Contains(lData.id))
                {
                    cinemaTypeMapCinemaIds[lData.type].Add(lData.id);
                }
                
                //缓存数据5
                List<int> tempList2;
                if (cinemaJsonData is CameraCinemaJsonData)
                {
                    int[] npcIds = (cinemaJsonData as CameraCinemaJsonData).hideNpcIds;
                    if (null != npcIds && npcIds.Length > 0)
                    {
                        for (int j = 0; j < npcIds.Length; j++)
                        {
                            int npcId = npcIds[j];
                            if (!npcIdMapCinemaIds.TryGetValue(npcId, out tempList2))
                            {
                                npcIdMapCinemaIds[npcId] = new List<int>();
                            }
                            npcIdMapCinemaIds[npcId].Add(lData.id);

                        }
                    }
                }



            }
        }



        //根据本地动画json导致，导入生成一个中间json数据
        public void Import()
        {
            //先读取本地数据
            _LoadAndCacheCinemaJsonData();
            // 生成中间json文件         
            int count = npcIdMapCinemaIds.Count;
            if (count > 0)
            {
                BridgeHideNpcCinemaJsonData bridgeHideNpcCinemaJsonData = new BridgeHideNpcCinemaJsonData();
                List<BridgeHideNpcCinemaJsonData.JsonData> datas = new List<BridgeHideNpcCinemaJsonData.JsonData>();
                foreach (int hideNpcId in npcIdMapCinemaIds.Keys)
                {
                    BridgeHideNpcCinemaJsonData.JsonData tempData = new BridgeHideNpcCinemaJsonData.JsonData();
                    tempData.npc_id = hideNpcId;
                    //data.cinema_type = CinemaType.None;
                    tempData.cinema_ids = npcIdMapCinemaIds[hideNpcId].ToArray();
                    datas.Add(tempData);
                }
                if (datas.Count > 0)
                {
                    bridgeHideNpcCinemaJsonData.data = datas.ToArray();
                    //导出成中间json,exe的当前目录
                    Persistence.json.Save(CinemaDataCommon.BridgeHideNpcCinemaJsonName, bridgeHideNpcCinemaJsonData);
                }

            }
        }

        //生成的中间json数据，导出新的动画json数据
        public void Export()
        {
            //1. 读取本地cinemjson数据
            _LoadAndCacheCinemaJsonData();
            //2. 读取中间json数据          
            BridgeHideNpcCinemaJsonData bridgeHideNpcCinemaJsonData = Persistence.json.LoadFile<BridgeHideNpcCinemaJsonData>(CinemaDataCommon.BridgeHideNpcCinemaJsonName);
            //3. 修改本地cinemaJson数据并导出成新的json文件
            List<int> _dataChangedCinemaIds = new List<int>();//动画配置数据发生变化了的动画ID
            if (null != bridgeHideNpcCinemaJsonData && (bridgeHideNpcCinemaJsonData.data.Length>0))
            {
                int count = bridgeHideNpcCinemaJsonData.data.Length;
                for (int i = 0; i < count; i++)
                {
                    BridgeHideNpcCinemaJsonData.JsonData jsonData = bridgeHideNpcCinemaJsonData.data[i];
                    int npcId = jsonData.npc_id;
                    CinemaType cinemType = jsonData.cinema_type;
                    int[] cinemaIdsTobeChanged = jsonData.cinema_ids;//收集需要修改的动画ID
                    //3 优先处理指定类型的动画，不然处理修改指定的动画ID
                    if (cinemType != CinemaType.None)
                    {
                        List<int> cinemaIdsWithSameType ;
                        if (cinemaTypeMapCinemaIds.TryGetValue(cinemType, out cinemaIdsWithSameType))
                        {                          
                            cinemaIdsTobeChanged = cinemaIdsWithSameType.ToArray();
                            
                        }
                    }
                    

                    foreach (var cinemaIdToBeChanged in cinemaIdsTobeChanged)
                    {
                        //根据动画ID获取对应的cinemaJsonData
                        CinemaJsonData cjd;
                        if (cinemaIdMapCinemaJsonData.TryGetValue(cinemaIdToBeChanged, out cjd))
                        {
                            if (cjd is CameraCinemaJsonData)
                            {
                                CameraCinemaJsonData ccjd = cjd as CameraCinemaJsonData;
                                //目前只有镜头动画才有隐藏npc的功能
                                //尝试追加npcId
                                bool dataChanged = _TryAddHideNpcId(ref ccjd, npcId);
                                if (dataChanged)
                                {
                                    //记录已经修改的动画ID
                                    if (!_dataChangedCinemaIds.Contains(cinemaIdToBeChanged))
                                    {
                                        _dataChangedCinemaIds.Add(cinemaIdToBeChanged);
                                    }
                                }
                            }
                        }
                    }

                    
                }
            }

            //4. 将数据有变化的动画数据导出为json
            foreach (var dataChangedCinemaId in _dataChangedCinemaIds)
            {
                //找到动画名
                string cinemaName = "";
                if (cinemaIdMapCinemaName.TryGetValue(dataChangedCinemaId, out cinemaName))
                {
                    //获取更新后的cinemaJsonData
                    CinemaJsonData cjd;
                    if (cinemaIdMapCinemaJsonData.TryGetValue(dataChangedCinemaId, out cjd))
                    {
                        //导出json文件,目前只修改镜头动画的数据
                        if (null != cjd && cjd is CameraCinemaJsonData)
                        {
                            Persistence.json.Save(CinemaDataCommon.GetCinemaJsonFileFullPath(cinemaName), cjd);
                            Console.WriteLine("modify cinema id:{0},cinema name:{1}",dataChangedCinemaId,cinemaName);
                        }

                    }

                }

            }
        }


        //确有添加操作则返回true
        public static bool _TryAddHideNpcId(ref CameraCinemaJsonData cameraCinemaJsonData, int npcId)
        {
            if (null != cameraCinemaJsonData)
            {
                int index = Array.IndexOf(cameraCinemaJsonData.hideNpcIds, npcId);
                if (index < 0)//不存在，则加入
                {
                    int[] newHideNpcIds = new int[cameraCinemaJsonData.hideNpcIds.Length+1];
                    cameraCinemaJsonData.hideNpcIds.CopyTo(newHideNpcIds, 0);
                    newHideNpcIds[cameraCinemaJsonData.hideNpcIds.Length] = npcId;//追加新的npcId
                    cameraCinemaJsonData.hideNpcIds = newHideNpcIds;
                    return true;
                }
            }
            return false;
        }
    }

}

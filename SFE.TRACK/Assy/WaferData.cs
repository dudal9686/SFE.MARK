using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreCSBase;
using CoreCSMac;
using CoreCSRunSim;

namespace DefaultBase
{
    public enum EnumRecipeDetailType
    {
        Not, WaferFlow, COT, DEV, ADH, LHP, HHP, CPL, TCP,
        WEE, EXP, SysTest,
        ItemCoat, ItemDevleop, ItemADH, ItemTemp, ItemTransition, ItemWEE, ItemExposure,
    };
    public enum WaferStorageWorkStep { IsNot, IsScanNeed, IsScanDone, IsRecipeNeed, IsRecipeDone };
    public class RecipeInfo
    {
        public string _Name = "";
        public EnumRecipeDetailType _RecipeType = EnumRecipeDetailType.Not;
        public WorkStep _WorkStep = WorkStep.IsNot;
        public int _BlockNo;
        public List<int> _moudleNoList = new List<int>();

        public void Init()
        {
            _Name = "";
            _RecipeType = EnumRecipeDetailType.Not;
            _WorkStep = WorkStep.IsNot;
            _BlockNo = -1;
        }

        public void SetInfo(string recipeName, EnumRecipeDetailType recipeType, WorkStep step, int blockNo, List<int> moduleNoList)
        {
            _Name = recipeName;
            _RecipeType = recipeType;
            _WorkStep = step;
            _BlockNo = blockNo;

            _moudleNoList.Clear();
            foreach (var id in moduleNoList) _moudleNoList.Add(id);
        }



    }
    public class WaferData
    {
        //public event EventHandler<LibEvtArgs> EvtDataChanged;

        public string _flowRecipeName;
        public bool _waferExist;
        public TLocation _locStart;
        public TLocation _locEnd;

        public string _WorkType;
        public int _BlockNo;
        public int _ModuleNo;
        public string _Occupier;
        public RecipeInfo[] _RecipeInfos = new RecipeInfo[30];
        public WaferData()
        {
            _Occupier = "";
            _WorkType = "";
            _BlockNo = -1;
            _ModuleNo = -1;

            for (int i = 0; i < 30; i++) _RecipeInfos[i] = new RecipeInfo();
        }


        public string GetInfomation(out int status)
        {
            status = 0;
            StringBuilder sb = new StringBuilder();
            if (_waferExist)
            {
                status = 2;
                sb.Append($"{_locStart.X + 1},{_locStart.Y + 1}");
                if (_flowRecipeName != "")
                {
                    status++;
                    sb.Append($" {_flowRecipeName} {_locEnd.X + 1},{_locEnd.Y + 1}");
                    RecipeInfo info = FindNextRecipeInfo();
                    if (info == null) return sb.ToString();
                    sb.AppendLine();

                    status = (int)info._WorkStep;
                    sb.AppendLine($"{info._Name}:{info._RecipeType.ToString()}:{info._WorkStep.ToString()}");

                    Tokenizer t = new Tokenizer();
                    foreach (var id in info._moudleNoList) t.AddCombineString(id.ToString());
                    sb.AppendLine($"B({info._BlockNo}) - M({t.CombineString(",")})");
                }
            }
            return sb.ToString();
        }

        public RecipeInfo FindNextRecipeInfo()
        {
            RecipeInfo info = null;

            for (int i = 0; i < _RecipeInfos.Length; i++)
            {
                info = _RecipeInfos[i];
                if (info._Name != "" && info._WorkStep <= WorkStep.IsDoing) return info;
            }

            return null;
        }


        public void Init()
        {
            _WorkType = "";
            _BlockNo = -1;
            _ModuleNo = -1;
        }
    }

    public class WaferDataArray
    {
        public WaferStorageWorkStep _waferStorageStep;
        private List<WaferData> _listWaferData = new List<WaferData>();
        //public event EventHandler<LibEvtArgs> EvtDataChanged;
        public WaferDataArray(int itemCount)
        {
            for (int i = 0; i < itemCount; i++)
            {
                _listWaferData.Add(new WaferData());
            }

        }

        public int GetWaferCount() { return _listWaferData.Count; }
        public WaferData GetWaferData(int index)
        {
            if (index >= _listWaferData.Count) return null;
            return _listWaferData[index];
        }

        public List<WaferData> GetList()
        {
            return _listWaferData;
        }

        public void Initialize()
        {
            foreach (WaferData waferData in _listWaferData)
            {
                waferData.Init();
            }
        }

    }
}

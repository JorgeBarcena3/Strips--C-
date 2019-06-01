using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.SampleMind
{
    class OperatorStrips //Un único operador! 
    {
        private CellInfo _cellInfo;

        public List<PropertyStrips> _pc;
        public List<PropertyStrips> _a;
        public List<PropertyStrips> _e;


        public OperatorStrips(List<PropertyStrips> pc, CellInfo target, string resultTag)
        {
            _cellInfo = target;
            _pc = pc;
            _a = new List<PropertyStrips> { new PropertyStrips(resultTag) };
        }

        public CellInfo GetCellInfo()
        {
            return _cellInfo;
        }
    }
}

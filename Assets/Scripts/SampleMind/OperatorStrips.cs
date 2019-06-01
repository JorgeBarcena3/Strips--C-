using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.SampleMind
{
    /// <summary>
    /// Conjunto de operadores
    /// </summary>
    public class Operators
    {

        public static List<OperatorStrips> _allOperators;

        public Operators(List<OperatorStrips> allOperatos)
        {
            Operators._allOperators = allOperatos;
        }

        private static Operators _instance;

        public static Operators Instance(List<OperatorStrips> _ops)
        {
          
                if (_instance == null) _instance = new Operators(_ops);
                return _instance;
            
        }

        public List<OperatorStrips> Produce(PropertyStrips p, List<OperatorStrips> Availables)
        {
            var r = new List<OperatorStrips>();
            foreach (var op in Availables)
            {
                if (op.Produce(p))
                {
                    r.Add(op);
                }
            }
            return r;
        }

    }

    /// <summary>
    /// Operador Unico
    /// </summary>
    public class OperatorStrips
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
            _e = new List<PropertyStrips>();
        }

        public CellInfo GetCellInfo()
        {
            return _cellInfo;
        }

        public virtual bool EsAplicable(EstadoStrips e)
        {
            foreach (var precon in _pc)
            {
                if (!e._actual.Contains(precon))
                {
                    return false;
                }
            }
            return true;
        }
        public virtual EstadoStrips Aplicar(EstadoStrips e)
        {
            var result = new EstadoStrips();
            foreach (var key in e._actual) 
            {
                result._actual.Add(key);
            }
            foreach (var r in _e)
            {
                result._actual.Remove(r);
            }
            foreach (var a in _a)
            {
                if (!result._actual.Contains(a))
                    result._actual.Add(a);
            }
            return result;

        }

        public bool Produce(PropertyStrips p)
        {
            return _a.Contains(p);
        }

    }
}

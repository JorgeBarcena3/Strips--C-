using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SampleMind
{
    public class PropertyStrips
    {
        public string _tag;

        public PropertyStrips(string tag)
        {
            _tag = tag;
        }

        public override bool Equals(object obj)
        {
            var o = obj as PropertyStrips;
            if (o == null) return false;
            return o._tag == this._tag;
        }

       
    }
}

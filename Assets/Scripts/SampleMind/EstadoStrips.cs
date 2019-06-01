using Assets.Scripts.SampleMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoStrips
{
    public List<PropertyStrips> _actual { get; private set; }

    public EstadoStrips()
    {
        _actual = new List<PropertyStrips>();
    }


    public EstadoStrips(List<PropertyStrips> props)
    {
        _actual = new List<PropertyStrips>();
        foreach (var p in props)
        {
            _actual.Add(p);
        }

    }


    public List<PropertyStrips> Pendientes(EstadoStrips estadoMeta)
    {
        var r = new List<PropertyStrips>();
        foreach (var p in estadoMeta._actual)
        {
            if (!_actual.Contains(p))
            {
                r.Add(p);
            }
        }
        return r;
    }

    public bool EsMeta(EstadoStrips estadoMeta)
    {
        return Pendientes(estadoMeta).Count == 0;
    }
}

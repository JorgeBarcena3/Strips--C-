using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.DataStructures;

public class Nodo
{

    public CellInfo estado;
    public Nodo nodoPadre;
    public Locomotion.MoveDirection direction;

    //f(n) = g(n) + h(n)
    public float f {
        get {
            return g + h_distanciaManhattan;
        }
    }
        
    public float g;
    public float h_distanciaManhattan;

    public Nodo(CellInfo _estado, Nodo _padre, Locomotion.MoveDirection _direction, float cost)
    {
        
        estado = _estado;
        nodoPadre = _padre;
        direction = _direction;
                     
    }

    public float heuristic(Vector2 a, Vector2 b) {

        var temp = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        return temp;

    }
    public bool esMeta()
    {

        if (estado.ItemInCell == null || estado.ItemInCell.Type != PlaceableItem.ItemType.Goal)
            return false;
        else return true;
    }

    public bool esMeta(CellInfo goal)
    {

        if (estado.ItemInCell == null || estado.ItemInCell.Tag != goal.ItemInCell.Tag)
            return false;
        else
            return true;
    }

    public bool esMeta(CellInfo goal, CellInfo posicionActual)
    {
        if (goal.GetPosition != posicionActual.GetPosition)
            return false;
        else return true;
    }

    public List<Nodo> ExpandirOffline(BoardInfo board)
    {

        var result = new List<Nodo>();
        var vecinos = estado.WalkableNeighbours(board);
        for (var i = 0; i < vecinos.Length; i++)
        {
            var vecino = vecinos[i];
            if (vecino != null)
            {
                var nuevo = new Nodo(vecino, this, (Locomotion.MoveDirection)i, vecino.WalkCost);
                result.Add(nuevo);

            }
        }

        return result;
    }



    public List<Nodo> ExpandirOnline(BoardInfo board)
    {

        List<EnemyBehaviour> Enemies = board.Enemies;
        var result = new List<Nodo>();
        var vecinos = estado.WalkableNeighbours(board);

        for (var i = 0; i < vecinos.Length; i++)
        {
            var vecino = vecinos[i];
            foreach (var enemy in Enemies)
            {
                if (vecino != null && vecino.CellId == enemy.CurrentPosition().CellId)
                {
                    vecino = null;
                }
            }
            if (vecino != null)
            {
                var nuevo = new Nodo(vecino, this, (Locomotion.MoveDirection)i, vecino.WalkCost);
                result.Add(nuevo);

            }
        }

        return result;
    }



    public Nodo getPadre()
    {

        return nodoPadre;
    }

}

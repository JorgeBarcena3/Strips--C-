using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using System.Linq;
using Assets.Scripts.DataStructures;
using UnityEngine;

public class Online : Assets.Scripts.AbstractPathMind
{
    //Lista de direcciones
    private List<Locomotion.MoveDirection> _nextMoves = null;
    //Posicion Inicial de nuestro player
    private Nodo posicionIncial;
    //Se ha acabado la lista de Nodos
    private bool listaCompleta = false;
    //Numeros de fases del algoritmo
    public enum Fases
    {
        FASE1,
        FASE2,
        FASE3
    }
    //Fase actual del algoritmo
    private Fases faseactual = Fases.FASE1;
    //Lista de nodos
    private List<Nodo> abierta = new List<Nodo>();
    //Limite de nodos
    public int limiteDeNodos;
    private int nodosActuales = 0;
    private bool aSearch = true;
    private List<Nodo> RoadNodos = new List<Nodo>();
    private int distanceEnemyNear = 0;

    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        distanceEnemyNear--;
        if (distanceEnemyNear < 1)
        {

            nodosActuales = 0;
            abierta.Clear();
            faseactual = Fases.FASE1;
            listaCompleta = false;
            _nextMoves = null;
            RoadNodos.Clear();
           
        }
       
        if (aSearch == true)
        {
            if (distanceEnemyNear < 1)
            {
                List<EnemyBehaviour> Enemies = boardInfo.Enemies;
                int[] enemyPosition = new int[2];
                int[] playerPosition = new int[2];
                foreach (var enemy in Enemies)
                {
                    enemyPosition[0] = enemy.CurrentPosition().ColumnId;
                    enemyPosition[1] = enemy.CurrentPosition().RowId;
                    if (distanceEnemyNear > distance(enemyPosition[0], enemyPosition[1], currentPos) || distanceEnemyNear < 1)
                        distanceEnemyNear = distance(enemyPosition[0], enemyPosition[1], currentPos);

                }
                aSearch = false;

            }
            
            //Si la lista no se ha completado, es decir, no se encuentra el nodo meta
            if (!listaCompleta && nodosActuales <= limiteDeNodos)
            {
                //Añadimos el nodo oficial
                if (faseactual == Fases.FASE1)
                {
                    abierta.Add(new Nodo(currentPos, posicionIncial, Locomotion.MoveDirection.None, 1f));
                    faseactual = Fases.FASE2;
                }

                //Nodo actual
                Nodo nodo = null;


                while (abierta.Count > 0 && !listaCompleta && nodosActuales <= limiteDeNodos)
                {
                    nodo = abierta[0];
                    abierta.RemoveAt(0);
                    if (nodo.esMeta())
                    {
                        listaCompleta = true;

                    }
                    else
                    {
                        var sucesores = nodo.ExpandirOnline(boardInfo);

                        foreach (var s in sucesores)
                        {
                            if (s != nodo.getPadre() && CheckAbiertaList(s, abierta))
                            {
                                abierta.Add(s);
                                nodosActuales++;
                            }
                        }
                    }

                }


                if (faseactual == Fases.FASE2)
                { 
                    RoadNodos = TakeRout(nodo, distanceEnemyNear);
                    _nextMoves = RoadNodos.Select(d=>d.direction).ToList();
                    faseactual = Fases.FASE3;
                }

            }

            if (searchEnemiesInRout(RoadNodos, boardInfo) && _nextMoves.Count > 0)
            {
                var currentMove = _nextMoves[_nextMoves.Count - 1];
                _nextMoves.RemoveAt(_nextMoves.Count - 1);
                aSearch = true;
                return currentMove;
            }
            else
            {
                return Locomotion.MoveDirection.None;
            }
           
        }
        else
        {
            return Locomotion.MoveDirection.None;
        }


    }

    public bool CheckAbiertaList(Nodo nodo, List<Nodo> abierta)
    {
        bool noRepetido = true;
        foreach (var s in abierta)
        {
            if (s.estado.CellId == nodo.estado.CellId)
                noRepetido = false;
        }
        return noRepetido;
    }
    public List<Nodo> TakeRout(Nodo meta, int nodos)
    {
        Nodo nodoActual = meta;
        List<Nodo> movements = new List<Nodo>();
        while (nodoActual.getPadre() != null)
        {

            movements.Add(nodoActual);
            nodoActual = nodoActual.getPadre();

        }
        if (movements.Count - 1- nodos > 0)
            movements.RemoveRange(0, movements.Count - 1 - nodos);
        else
            movements.RemoveRange(0, movements.Count - 1);

        return movements;

    }

    public bool searchEnemiesInRout(List<Nodo> RoutNodos, BoardInfo boardInfo)
    {

        List<EnemyBehaviour> Enemies = boardInfo.Enemies;

        foreach (var enemy in Enemies)
        {
            foreach (var nodo in RoutNodos)
            {
                if (enemy.CurrentPosition().CellId == nodo.estado.CellId)
                {

                    return false;
                }
            }
        } 
        
        return true;

    }

    private int distance(int x, int y, CellInfo currentPosition)
    {
        return (int)(Mathf.Abs(currentPosition.ColumnId - x) + Mathf.Abs(currentPosition.RowId - y)/2); 
    }


    public override void Repath()
    {
        throw new System.NotImplementedException();
    }


    // Update is called once per frame
    void Update()
    {

    }
}

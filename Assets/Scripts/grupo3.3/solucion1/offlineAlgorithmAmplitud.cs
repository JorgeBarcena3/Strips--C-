using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using System.Linq;
using Assets.Scripts.DataStructures;
using UnityEngine;



public class offlineAlgorithmAmplitud : Assets.Scripts.AbstractPathMind
{
    //Lista de direcciones
    private List<Locomotion.MoveDirection> _nextMoves = null;
    //Posicion Inicial de nuestro player
    private Nodo posicionIncial;
    //Se ha acabado la lista de Nodos
    private bool listaCompleta = false;
    //Numeros de fases del algoritmo
    public enum Fases {
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


    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        //Si la lista no se ha completado, es decir, no se encuentra el nodo meta
        if (!listaCompleta && nodosActuales <= limiteDeNodos)
        {
           
            //Añadimos el nodo oficial
            if (faseactual == Fases.FASE1) {
                abierta.Add(new Nodo(currentPos, posicionIncial, Locomotion.MoveDirection.None, 1f));
                faseactual = Fases.FASE2;
            }

            //Nodo actual
            Nodo nodo = null;


            while (abierta.Count > 0 && !listaCompleta && nodosActuales <= limiteDeNodos)
            {
                nodo = abierta[0];
                abierta.RemoveAt(0);
                if (nodo.esMeta(goals[0]))
                {
                    listaCompleta = true;
                    Debug.Log("Nodos expandidos: " + nodosActuales);

                }
                else
                {
                    var sucesores = nodo.ExpandirOffline(boardInfo);

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
                _nextMoves = TakeRout(nodo);
                faseactual = Fases.FASE3;
            }

        }


        var currentMove = _nextMoves[_nextMoves.Count - 1];
        _nextMoves.RemoveAt(_nextMoves.Count - 1);
        return currentMove;

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
    public List<Locomotion.MoveDirection> TakeRout(Nodo meta)
    {
        Nodo nodoActual = meta;
        List<Locomotion.MoveDirection> movements = new List<Locomotion.MoveDirection>();
        while (nodoActual.getPadre() != null)
        {

            movements.Add(nodoActual.direction);
            nodoActual = nodoActual.getPadre();

        }

        return movements;

    }


    public override void Repath()
    {
        listaCompleta = false;
        faseactual = Fases.FASE1;
        abierta = new List<Nodo>();
        nodosActuales = 0;
    }


    // Update is called once per frame
    void Update()
    {

    }
}


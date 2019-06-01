using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using System.Linq;
using Assets.Scripts.DataStructures;
using UnityEngine;



public class offlineAlgorithmAStar : Assets.Scripts.AbstractPathMind
{
    public int maxNodos;
    private int currentNodos;
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
    //Lista abierta de nodos
    private List<Nodo> abierta = new List<Nodo>();
    //Lista de todos los nodos
    private List<Nodo> cerrada = new List<Nodo>();
    //Lista de movimientos finales
    [HideInInspector]
    public List<Locomotion.MoveDirection> ruta = new List<Locomotion.MoveDirection>();
    //Posicion Inicial de nuestro player
    private Nodo posicionIncial;


    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        if (faseactual == Fases.FASE1)
        {
            posicionIncial = new Nodo(currentPos, posicionIncial, Locomotion.MoveDirection.None, currentPos.WalkCost);
            posicionIncial.g = 0;
            posicionIncial.h_distanciaManhattan = posicionIncial.heuristic(posicionIncial.estado.GetPosition, boardInfo.Exit.GetPosition);
            posicionIncial.nodoPadre = null;
            abierta.Add(posicionIncial);
            faseactual = Fases.FASE2;
        }

        if (faseactual == Fases.FASE2)
        {

            Nodo NodoActual = null;

            //Si la lista no se ha completado, es decir, no se encuentra el nodo meta
            while (abierta.Count != 0 && !listaCompleta)
            {
                //Nodo actual
                abierta = abierta.OrderBy(node => node.f).ToList();
                NodoActual = abierta[0];
                abierta.Remove(NodoActual);
                cerrada.Add(NodoActual);

                if (NodoActual.esMeta(goals[0]))
                {
                    listaCompleta = true;
                    ruta = TakeRout(NodoActual);
                    Debug.Log("Nodos expandidos: " + currentNodos);
                    faseactual = Fases.FASE3;

                }
                else
                {

                    var sucesores = NodoActual.ExpandirOffline(boardInfo);
                    foreach (var vecino in sucesores)
                    {
                        if (!cerrada.Contains(vecino))
                        {

                            vecino.nodoPadre = NodoActual;
                            vecino.h_distanciaManhattan = vecino.heuristic(vecino.estado.GetPosition, boardInfo.Exit.GetPosition);
                            vecino.g = vecino.estado.WalkCost + vecino.nodoPadre.g;


                            abierta.Add(vecino);
                            currentNodos++;

                        }

                    }
                }



                if (currentNodos > maxNodos)
                    listaCompleta = true;


            }


        }


        if (faseactual == Fases.FASE3 && currentNodos < maxNodos)
        {

            var currentMove = ruta[ruta.Count - 1];
            ruta.RemoveAt(ruta.Count - 1);
            return currentMove;
        }


        return Locomotion.MoveDirection.None;
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
        currentNodos = 0;
    }


    // Update is called once per frame
    void Update()
    {

    }
}


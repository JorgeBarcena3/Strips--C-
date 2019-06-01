using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.SampleMind;
using UnityEngine;

namespace Assets.Scripts
{

    public struct ResultStrips
    {
        public EstadoStrips Estado;
        public List<OperatorStrips> Plan;
        public bool Valido;
    }

    public class Planner : MonoBehaviour
    {

        private CharacterBehaviour _character;

        private List<OperatorStrips> _currentPlan;

        private List<OperatorStrips> _allOperators;

        private OperatorStrips _estadoFinal;

        public int contador_operaciones = 0;

        private System.Random r = new System.Random(100);


        public void Start()
        {
            _currentPlan = new List<OperatorStrips>();
            _allOperators = new List<OperatorStrips>();
        }

        public void SetCharacter(CharacterBehaviour character)
        {
            _character = character;

        }

        public CellInfo GetNextAction()
        {
            if (_allOperators.Count == 0)
            {
                var l = _character.BoardManager.boardInfo.ItemsOnBoard;
                foreach (PlaceableItem p in l)
                {
                    var lPC = new List<PropertyStrips>();
                    foreach (var pc in p.Preconditions)
                    {
                        lPC.Add(new PropertyStrips(pc.Tag));
                    }

                    var target = _character.BoardManager.boardInfo.CellWithItem(p.Tag);
                    _allOperators.Add(new OperatorStrips(lPC, target, p.Tag));
                }

                Debug.Log("Final");
            }

            if (_currentPlan.Count == 0)
            {
                //Obtenemos el estado final
                _estadoFinal = _allOperators.Where(x => x._a[0]._tag == "Goal").First();
                //Ordenamos el array en funcion de los objetivos
                _estadoFinal._pc.OrderBy(m =>
                (_allOperators.Where(x => x._a[0]._tag == m._tag).First()._pc.Count)
                );
                //Generamos el estado Meta
                var estadoMeta = new EstadoStrips(_estadoFinal._pc);
                //Iniciacion del documento
                var init = new ResultStrips
                {
                    Estado = new EstadoStrips(),
                    Plan = new List<OperatorStrips>(),
                    Valido = true
                };

                ///////////////////////////////////////////////////////////
                ///////////////////ALGORITMO DE STRIPS/////////////////////
                ///////////////////////////////////////////////////////////

                //Obtenemos el resultado
                var result = StripsHeuristica(init, estadoMeta);
                //Lo metemos en el currentPlan
                _currentPlan = result.Plan;
                //Añadimos el estado final
                _currentPlan.Add(_estadoFinal);

                ///////////////////////////////////////////////////////////
                //ESTE ES EL OTRO ALGORITMO DE PLANIFICACION DE OBJETIVOS//
                ///////////////////////////////////////////////////////////

               // ListaDeEstados(_estadoFinal); //Descomentar para utilizar esta opcion


                //Estadisticas
                Debug.Log("Numero de operaciones " + contador_operaciones + " || PC Goal: " + _estadoFinal._pc.Count);
            }

            var nextOperation = _currentPlan[0];
            _currentPlan.RemoveAt(0);
            return nextOperation.GetCellInfo();
        }


        /// <summary>
        /// Algoritmo regresivo
        /// </summary>
        private void ListaDeEstados(OperatorStrips estadoMeta)
        {
            //Estadisticas
            contador_operaciones++;

            _estadoFinal._pc.OrderBy(m =>
                (_allOperators.Where(x => x._a[0]._tag == m._tag).First()._pc.Count)
                );

            foreach (var _pc in estadoMeta._pc)
            {

                var _estadoAuxialiar = _allOperators.Where(x => x._a[0]._tag == _pc._tag).First();
                ListaDeEstados(_estadoAuxialiar);
            }

            if (!_currentPlan.Contains(estadoMeta))
                _currentPlan.Add(estadoMeta);

        }

        /// <summary>
        /// Algoritmo de strips
        /// </summary>
        public ResultStrips StripsHeuristica(ResultStrips init, EstadoStrips estadoMeta)
        {
            contador_operaciones++;

            while (init.Estado.Pendientes(estadoMeta).Count > 0)
            {
                var meta = ElegirMeta(init.Estado.Pendientes(estadoMeta));
                var operadoresCandidatos = Operators.Instance(_allOperators).Produce(meta, new List<OperatorStrips>(_allOperators));

                while (operadoresCandidatos.Any())
                {
                    var operador = ElegirOperador(new List<OperatorStrips>(operadoresCandidatos));
                    var newResult = StripsHeuristica(new ResultStrips
                    {
                        Estado = init.Estado,
                        Plan = init.Plan
                    },
                    new EstadoStrips(operador._pc));

                    if (newResult.Valido)
                    {

                        if (!init.Plan.Contains(operador))
                        {

                            init.Estado = operador.Aplicar(init.Estado);
                            init.Plan.Add(operador);
                        }
                        else
                            init.Estado = operador.Aplicar(init.Estado);

                        break;
                    }
                }
                if (!operadoresCandidatos.Any())
                {
                    init.Valido = false;
                    return init;
                }
            }
            if (init.Estado.Pendientes(estadoMeta).Count == 0)
            {
                init.Valido = true;

            }
            else
            {
                init.Valido = false;
            }
            return init;
        }

        private OperatorStrips ElegirOperador(List<OperatorStrips> candidatos)
        {

            var i = r.Next(candidatos.Count);
            var op = candidatos[i];
            candidatos.RemoveAt(i);
            return op;
        }

        public PropertyStrips ElegirMeta(List<PropertyStrips> pendientes)
        {
            int i = r.Next(pendientes.Count);

            return pendientes[i];
        }

    }
}

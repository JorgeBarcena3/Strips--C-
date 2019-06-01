using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.SampleMind;
using UnityEngine;

namespace Assets.Scripts
{
    public class Planner : MonoBehaviour
    {

        private CharacterBehaviour _character;

        private List<OperatorStrips> _currentPlan;

        private List<OperatorStrips> _allOperators;

        private OperatorStrips _estadoFinal;

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
                _estadoFinal._pc.OrderBy(m =>
                (_allOperators.Where(x => x._a[0]._tag == m._tag).First()._pc.Count)
                );

                ListaDeEstados(_estadoFinal);
                if (_currentPlan[_currentPlan.Count - 1]._a[0]._tag != "Goal")
                    _currentPlan.Add(_estadoFinal);

            }

            var nextOperation = _currentPlan[0];
            _currentPlan.RemoveAt(0);
            return nextOperation.GetCellInfo();
        }

        private void ListaDeEstados(OperatorStrips estadoMeta)
        {

            foreach (var _pc in estadoMeta._pc)
            {

                var _estadoAuxialiar = _allOperators.Where(x => x._a[0]._tag == _pc._tag).First();
                ListaDeEstados(_estadoAuxialiar);

            }

            if (!_currentPlan.Contains(estadoMeta))
                _currentPlan.Add(estadoMeta);

        }

    }
}

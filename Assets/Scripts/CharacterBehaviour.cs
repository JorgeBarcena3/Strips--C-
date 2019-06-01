using Assets.Scripts.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Locomotion))]
    public class CharacterBehaviour : MonoBehaviour
    {

        protected Locomotion LocomotionController;
        protected AbstractPathMind PathController;
        public BoardManager BoardManager { get; set; }

        protected CellInfo currentTarget;

        //Online Variables
        public bool online;
        //Tiempo de decision
        public int tiempoDePensamiento = 3;
        private int currentTiempoPensamiento = -2;
        private List<GameObject> enemies = new List<GameObject>();
        private bool ObjetivoConseguido = false;

        //Planner Variables
        public Planner PlanController { get; private set; }



        void Start()
        {

            PathController = GetComponentInChildren<AbstractPathMind>();
            PathController.SetCharacter(this);
            LocomotionController = GetComponent<Locomotion>();
            LocomotionController.SetCharacter(this);

            //Online
            var aux = GameObject.FindGameObjectsWithTag("Enemy");
            enemies = new List<GameObject>(aux);

            //Planner
            PlanController = GetComponentInChildren<Planner>();
            PlanController.SetCharacter(this);


        }

        void Update()
        {

            if (BoardManager == null) return;

            //Si el modo de juego es del planner
            if (this.currentTarget == null)
            {
                CellInfo nextAction = PlanController.GetNextAction();
                this.SetCurrentTarget(nextAction);
                PathController.Repath();
            }

            //Si el modo de juego en online
            if (online)
            {

                if (esMeta(LocomotionController.CurrentEndPosition()))
                {
                    enemies.RemoveAt(0);
                    currentTiempoPensamiento = tiempoDePensamiento;
                    ObjetivoConseguido = true;
                }

                if (LocomotionController.MoveNeed || ObjetivoConseguido)
                {
                    currentTiempoPensamiento++;
                    ObjetivoConseguido = false;

                    if (currentTiempoPensamiento < 0)
                    {

                        changeTarget();
                        var boardClone = (BoardInfo)BoardManager.boardInfo.Clone();
                        LocomotionController.SetNewDirection(PathController.GetNextMove(boardClone, LocomotionController.CurrentEndPosition(), new[] { this.currentTarget }));

                        return;
                    }


                    if (currentTiempoPensamiento >= tiempoDePensamiento)
                    {
                        currentTiempoPensamiento = 0;
                        changeTarget();
                        var boardClone = (BoardInfo)BoardManager.boardInfo.Clone();
                        PathController.Repath();
                        LocomotionController.SetNewDirection(PathController.GetNextMove(boardClone, LocomotionController.CurrentEndPosition(), new[] { this.currentTarget }));
                        return;

                    }
                    else {
                        var boardClone = (BoardInfo)BoardManager.boardInfo.Clone();
                        LocomotionController.SetNewDirection(PathController.GetNextMove(boardClone, LocomotionController.CurrentEndPosition(), new[] { this.currentTarget }));

                    }


                }
            }
            else
            {             

                //Si el modo de juego es offline
                if (LocomotionController.MoveNeed)
                {
                    var boardClone = (BoardInfo)BoardManager.boardInfo.Clone();
                    LocomotionController.SetNewDirection(PathController.GetNextMove(boardClone, LocomotionController.CurrentEndPosition(), new[] { this.currentTarget }));

                    if (this.currentTarget == LocomotionController.CurrentEndPosition())
                    {
                        SetCurrentTarget(null);
                    }
                }
            }
        }



        public void SetCurrentTarget(CellInfo newTargetCell)
        {
            this.currentTarget = newTargetCell;
        }

        public void changeTarget()
        {
            if (enemies.Count > 0)
            {
                this.currentTarget = enemies[0].GetComponent<EnemyBehaviour>().CurrentPosition();
            }
            else
            {
                this.currentTarget = BoardManager.boardInfo.Exit;
            }
        }

        public bool esMeta(CellInfo currentPos)
        {
            if (currentPos.GetPosition == this.currentTarget.GetPosition)
                return true;

            return false;
        }
    }
}


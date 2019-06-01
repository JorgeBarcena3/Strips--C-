using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts
{
    public class ItemLogic : MonoBehaviour
    {

        public string Tag {get { return PlaceableItem!=null?PlaceableItem.Tag:""; } }
        
        public Sprite ActiveSprite;

        public PlaceableItem.ItemType Type;
        
        public PlaceableItem PlaceableItem { get; set; }
        
        public string RequieredTags
        {
            get
            {
                var str = "Reqs: ";
                return PlaceableItem.Preconditions.Aggregate(str,(curr, next) => curr + ", " + next.Tag);

            }
        }

        public void Start()
        {
            Debug.Log(Tag+ ">"+ RequieredTags);
            
        }
        void OnTriggerEnter2D(Collider2D collider2D)

        {
            if (!collider2D.gameObject.CompareTag("Player")) return;
            if (PlaceableItem.Preconditions.Any(o => !o.Activated))
            {
                return;
            }
            var render = GetComponent<SpriteRenderer>();
            render.sprite = ActiveSprite;
            PlaceableItem.Activated = true;


            if (this.Type == PlaceableItem.ItemType.Goal)
                 QuitGame();
            if (this.Type== PlaceableItem.ItemType.Enemy)
                GameObject.Destroy(gameObject,0.5f);
        }

        public void QuitGame()
        {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
        void OnGUI()
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(pos.x-25,Screen.height-pos.y,70,20), Tag );
            GUI.Label(new Rect(pos.x - 25, Screen.height - pos.y - 20, 370, 20), RequieredTags);
        }
    }
}

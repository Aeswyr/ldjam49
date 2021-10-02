using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Unstable
{
    [RequireComponent(typeof(Tile))]
    public class PuddleTile : MonoBehaviour, ITile
    {
        private Tile _tile; // Tile injection

        private void Awake()
        {
            _tile = this.GetComponent<Tile>();
        }

        #region ITile

        public Tile.Type GetTileType()
        {
            return _tile.GetTileType();
        }

        public void OnEnter(ref Slidable slidable)
        {
            Debug.Log("Entered puddle tile.");
            slidable.Shunt();
        }

        public void OnExit(ref Slidable slidable)
        {
            Debug.Log("Exited puddle tile.");
            slidable.EndShunt();
        }

        #endregion
    }
}
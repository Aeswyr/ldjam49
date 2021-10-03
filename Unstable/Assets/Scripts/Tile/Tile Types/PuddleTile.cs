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
            slidable.Shunt();
            slidable.MultiplySlideSpeed(0.5f);
        }

        public void OnExit(ref Slidable slidable)
        {
            // now called when timer ends / collision occurs
            // slidable.EndShunt();
            slidable.MultiplySlideSpeed(2f);
        }

        #endregion
    }
}
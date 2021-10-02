using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    [RequireComponent(typeof(Tile))]
    public class WoodTile : MonoBehaviour, ITile
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
            Debug.Log("Entered wood tile.");
        }

        public void OnExit(ref Slidable slidable)
        {
            Debug.Log("Exited wood tile.");
        }

        #endregion
    }
}
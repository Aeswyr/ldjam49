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

        public void ApplyEffect()
        {
            Debug.Log("you are on wood.");
        }

        public Tile.Type GetTileType()
        {
            return _tile.GetTileType();
        }

        #endregion
    }
}
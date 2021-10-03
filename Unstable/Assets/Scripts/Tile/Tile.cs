using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Unstable
{
    public class Tile : MonoBehaviour, ITile
    {
        public enum Type
        {
            wood,
            ice,
            puddle,
            none
        }

        [SerializeField]
        private Tile.Type m_type;

        #region Accessors

        public Tile.Type GetTileType()
        {
            return m_type;
        }

        public void OnEnter(ref Slidable slidable)
        {
            throw new System.NotImplementedException();
        }

        public void OnExit(ref Slidable slidable)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

}

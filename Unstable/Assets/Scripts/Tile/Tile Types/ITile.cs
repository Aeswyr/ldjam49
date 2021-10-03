using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    public interface ITile
    {
        Tile.Type GetTileType();

        void OnEnter(ref Slidable slidable);

        void OnExit(ref Slidable slidable);
    }
}

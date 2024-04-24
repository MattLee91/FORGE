using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Building", menuName = "Tiles/Bounce")]
public class PlatformBounce : ScriptableObject
{
   public Vector3Int cell0;
   public TileBase tile0;
   public Vector3Int cell1;
   public TileBase tile1;

   public Vector3Int cell2;
   public TileBase tile2;

    public Vector3Int cell3;
   public TileBase tile3;

}

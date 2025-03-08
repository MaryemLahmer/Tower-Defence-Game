using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "GridCell", menuName = "TowerDefenceGame/GridCell")]
public class GridCellObject : ScriptableObject
{
   public enum CellType {Path, Ground}
   public GameObject cellPrefab;
   public CellType cellType;
   public int yRotation;
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 빈 타일, 길 타일, 기믹 타일(길을 항상 포함)
public enum TileType
{
    None,
    Road,
    Gimmick
}

// 타일 모양(빈 타일의 경우 None, 순서대로 선, L자, T자, 십자, 출발점, 종료점)
public enum RoadShape
{
    None,
    Straight,
    L,
    T,
    Cross,
    Start,
    End
}

// 기믹 모양(기믹 없는 타일의 경우 None
public enum GimmickShape
{
    None,
    Gimmick1,
    Gimmick2
}

public struct Tile
{
    public TileType Type; // 빈 타일, 길 타일, 기믹 타일
    public RoadShape RoadShape;
    public GimmickShape GimmickShape;
    public int RotateValue;
}
public class TileNode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using Unity.Entities;
using Unity.Mathematics;



public struct SpawnerData : IComponentData
{
    public Entity Prefab;
    public int SpawnCount;
    public float SpawnRadius;
    public float3 CenterPosition;
    public float ScaleFactor;
    public quaternion BaseRotationOffset;
    
    // 新增：記錄生成器本身的旋轉，用來決定半圓形面向哪裡
    public quaternion SpawnerRotation; 
}


public struct SpectatorPrefabElement : IBufferElementData
{
    public Entity ListPrefab;
}
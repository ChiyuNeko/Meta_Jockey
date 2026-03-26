using Unity.Entities;

// 這是一個標記與資料結構，告訴 ECS 這個 Entity 是一個觀眾
public struct SpectatorData : IComponentData
{
    // 你可以在這裡放任何與觀眾有關的資料，例如他目前的心情、手持螢光棒的 ID 等
    // 目前我們先放一個簡單的變數作為範例
    public float RandomSpeedOffset; 
}
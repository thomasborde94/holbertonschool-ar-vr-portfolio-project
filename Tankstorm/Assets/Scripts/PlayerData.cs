using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable 
{
    public ulong clientId;
    public int roleId;
    public FixedString64Bytes playerName;

    public bool Equals(PlayerData other)
    {
        return
            clientId == other.clientId &&
            roleId == other.roleId &&
            playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref roleId);
        serializer.SerializeValue(ref playerName);
    }
}

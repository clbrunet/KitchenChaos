using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString64Bytes name;
    public FixedString64Bytes id;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId && name == other.name && id == other.id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref id);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerBullet : IComponentData
{
   [GhostDefaultField]
    public int PlayerId;

}

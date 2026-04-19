using System;
using Unity.Behavior;

[Flags]
[BlackboardEnum]
public enum NPCState
{
	Calm = 1 << 0,
	Flee = 1 << 1
}

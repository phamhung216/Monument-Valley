using System;

[Flags]
public enum NavAccessFlags
{
	Player = 1,
	Crow = 2,
	Totem = 4,
	Autowalker = 8,
	NotBlocked = 0x10,
	CrowGrounded = 0x20
}

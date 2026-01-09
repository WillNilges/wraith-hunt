# Wraith Hunt 

A Demon haunts an apartment complex. He feeds on the innocent. You must stop him.

An asymmetrical split-screen competitve arcade platformer that requires locating and eliminating your opponent.


Concepts:
```
Medium vs. Demon

Medium sees material plane, demon sees ethereal plane

Neither can see the other on their own screen

Demon sets traps for Medium, Medium has to avoid them, find the demon, and exorcise it. This is done by screen cheating. Both players are allowed and _encouraged_ to screen cheat.

Medium's abilities consist of beam attacks, shields, and astral projection.
    When the Medium astral projects, she has the ability to locate the demon, disarm traps, and rip the demon out of a posession, the screens are joined together, and the players fight in the ethereal plane.
    The Medium is more vulnerable here, taking increased damage. If she is killed here, it's game over.

Demon's abilities consist of AoE traps, flinging furniture, and posessing people.
    When the demon possesses someone, the screens are joined together, and the players fight in the material plane.
        While the demon is posessing somone, he has powerful melee attacks and gravity manipulation

The setting: A massive, cyberpunk apartment complex. Multiple floors, platforming, and stuff to interact with.

If the Medium rips the demon out of posession, he is trapped in a small chunk of the ethereal plane for a time, and the Medium does extra damage.

She can kill the demon by fighting him on the ethereal plane, or ripping him out of hosts and fighting him.

The demon can win by by killing the Medium, or by posessing and killing enough people. She is vulnerable to attacks on the material plane (throwing shit at her, melee attacks, and traps) and on the ethereal plane.
```

# Development

Tiling software: (Tiled) https://www.mapeditor.org/
Sound software: https://www.bfxr.net/

Monogame Editor

See also: https://github.com/ComputerScienceHouse/devcade-onboard/blob/main/HACKING/Dockerfile

# Running dev versions of the game

### Software Requirements
- dotnet

To run the game:

`dotnet publish -c Release -r linux-x64 --self-contained`

nuget?

`dotnet add package devcade-library`


`DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 ./DevcadeGame`

`dotnet run`

# libfreetype compilation issues

This is cursed.

The nix flake should have a copy of libfreetype in it. Find one in your system with
`find /nix/store/ -name "libfreetype.so"`

And then do

`ln -s /nix/store/l1g7r8sgrc3djn0x51mpy8fz5wp3204d-freetype-2.13.3/lib/libfreetype.so /home/wilnil/.nuget/packages/dotnet-mgcb/3.8.1.303/tools/net6.0/any/runtimes/linux-x64/native/freetype6.so`




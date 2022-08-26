# GJhamEngine

Actually a library because the UI will probably look 15 years old, executable by BATs and there will be more than one editor........

Still in development, any help is appreciated, if you wanna help please enter my dev discord server and talk to me. https://discord.gg/NPfbwZTd

# STATUS

-> Can finally be effectively used as some sort of deterministic framework library, but it may cause a lot of PAIN to do so.

# Current tasks to be done (skip to goals if you're new)

-> Start fase 3, aka making the actual entities that will be used for production in the future. (NOTE TO SELF: use main class for main 'collision hub')

-> Make prefab system. (NOTE TO SELF: to deserialize store each entity in a array and then add children according to their index in the array)

-> Make editor.

# Phylosophy

Ticks (the part where the program processes inputs and movement) must be executed as perfectly and without performance issues as possible; everything else is background loading, even rendering.

# Goals from BIG-BIG-BIG to less important

0. All is free. (probably will have donation page if people actually become interested in this project)

1. Completely deterministic unless the user messes up.

2. Fixing an issue with float where rendering gets distorted the higher your coordenates are. (this will be done by calculating the final rendering position with the camera before casting to float)

3. A built-in... multiple server hosting system so you can host more than one server in the same port. (for tournaments in fighting games or if somehow you want to host a 2d MMO in this, i don't think you should... but hey, let's make that easier, it might be good for someone with a inovative ideia... and not an MMO)

4. Prefab-like system.

5. No goddam boxing. (it's bad for memory usage and C# GC is bad with big memory)

6. Bearable performance.

7. Editor(s).

8. Fix my own mistakes. (i know, it's hard, but this rendering system is making me question myself as a programmer, it allocates way too much memory and probably ends up in gen 2. EDIT june 04 y22: now it's better)

9. ~~Probably 3d after i finish 2d.~~ (forget that, that won't be possible)

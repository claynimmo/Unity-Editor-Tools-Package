
Prefab Brush:
Paint prefab objects onto the world, replicating the tree brush tool for terrains. The tool is found as a window in the tools menu at the top of the editor. You can set a transform to act as a parent, otherwise it defaults to "Painted Prefabs". Fully randomize rotation rotates all axis, when it normaly only rotates along the y axis. Randomize scale scales uniformly (all axis scale by the same amount). To enable painting, active the enable painting bool in the window.

Custom rotate tool:
rotates like normal, but includes a cube in the middle that gives a completely random rotation when pressed

Child move tool:
position the parent like normal, but also include cardinal direction only movement gizmos to every child. This can help in positioning empty objects used as waypoints

Neighbors tool:
When selecting a transform, the tool becomes available. It creates a bounding box around the object and its children's mesh filters. Pressing on one of the arrows duplicates the object, and aligns it perfectly to that face. This tool is helpful for perfectly aligning wall models without overlapping UVs.

Belzier Curve:
A belzier curve is included in the package. Add the BelzierPath script to any gameobject to create the path. On selecting it, a tool becomes available where all of the vertices can be moved individually, adjusting the curve. A sample script to follow this path has been provided.
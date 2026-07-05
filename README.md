
## Keybind List

- alt+b: shortcut to prefab brush tool ([see Toolbar Tools](#toolbar-tools))
- alt+r: shortcut to custom rotate tool ([see Toolbar Tools](#toolbar-tools))
- alt+x: shortcut to child move tool ([see Toolbar Tools](#toolbar-tools))
- alt+n: shortcut to neighbors tool ([see Toolbar Tools](#toolbar-tools))
- alt+m: shortcut to measure tool ([see Toolbar Tools](#toolbar-tools))
- ctrl+alt+s: save current selection ([see Hierarchy/Selection Tools](#selection))

## Toolbar Tools
**Prefab Brush**:
Paint prefab objects onto the world, replicating the tree brush tool for terrains. The variables are adjusted from a popup window, where painting cannot be done until at least the prefab is set. You can set a transform to act as a parent, otherwise it defaults to an object (already existing, or new) called "Painted Prefabs". Fully randomize rotation rotates all axis, when it normaly only rotates along the y axis. Randomize scale scales uniformly (all axis scale by the same amount). To enable painting, active the enable painting bool in the window.

**Custom rotate tool:**
rotates like normal, but includes a cube in the middle that gives a completely random rotation when pressed.

**Child move tool:**
position the parent like normal, but also include cardinal direction only movement gizmos to every child. This can help in positioning empty objects used as waypoints, or moving invisible walls relative to each other.

**Neighbors tool:**
When selecting a transform, the tool becomes available. It creates a bounding box around the object and its children's mesh filters. Pressing on one of the arrows duplicates the object, and aligns it perfectly to that face. This tool is helpful for perfectly aligning wall models without overlapping UVs.

**Measure tool:**
measure the distance between multiple different points. Pressing x y or z locks the movement in that direction, similar to the controls in blender. Left click places the marker where the mouse is raycast, and right click removes all markers. The tool opens a window for additional settings, which includes the distance threshold. The threshold changes the measurement line to red when it exceeds the distance specified by the threshold. Use this tool against known distance parameters, like jump height, to test if the level design is usable without needing to continuously enter play mode to test.

**Bezier Curve:**
A bezier curve is included in the package. Add the BelzierPath script to any gameobject to create the path. On selecting it, a tool becomes available where all of the vertices can be moved individually, adjusting the curve. A sample script to follow this path has been provided.

## Hierarchy Tools

### Right-click Tools
These tools appear in the popup when right clicking on a gameobject in the hierarchy. The tools are found under the Tools folder, usually located under prefabs.

#### Parent
There are several tools regarding managing parents. Right clicking an object with children gives the commands:
 
- **Centre to Children**: Move the selected objects origin to the centre of its children as defined by the centre pivot mode.
- **Reset Scale/Rotation**: Reset the scale or rotation of the parent without impacing the childing, useful for when the selected parent is an empty object with a non-uniform transform, negatively impacting the children (such as distortions when rotating the child).
- **Enable/Disable Children**: under the heading *Set Active* lies the action to enable or disable all children of the selected gameobject. While this is not as usefull compared to simply group selecting them all, it is included to fully round the toolset.

Right clicking an object with a parent gives the commands:

- **Move Parent to This**: moves the origin of the parent to the origin of the selected object, without moving the other children. This simplifies the worflow of unparenting all children from object X, parenting object X into target child Z, reset position, unparent X from Z, then reparent all children back into X.
- **Swap with Parent**: Swap the selected child and its parent in the hierarchy. For example, if object A has children B and C, and you select this command from object B, the hierarchy becoms object B with children A and C. The order is preserved, and the children of the swapped component is transferred (so object A has child B, who has child D, and when swapping A and B, it becomes B->A->D).

#### Transform

- **UV Shift (all 6 cardinal directions)**: slightly shift the objects local position (for example transform.up), by such a small margin that it removes the problem of overlapping UVs.

- **UV Scale**: slightly increase or decrease local scale uniformely by a negligable factor, to remove overlapping UVs.

#### Selection
A set of tools that appear when one or more gameobjects are selected in the hierarchy.

- **Save Selected Objects**: Save all selected objects into a static cache, to be used in other tools. It essentially acts as storing the multi-selection in a form where it is not reset when clicking away.

### Visuals
The visuals of the hierarchy menu is adjusted to draw an icon next to the gameobjects, to make it easier to visually see the purpose of an object. It includes preferences to enable or disable any of the icons. The blue and pink diamonds represent invisible and empty objects respectively.

## Context Tools
Context tools appear when right clicking on serialized fields or components from the inspector when selected on a gameobject.

Tools that appear when right clicking a component:

### List/Array:

- **Fill from Saved Selection**: appears when the static cache saved through the *Save Selected Objects* right-click selection tool. It fills the collection using the stored object references. For example, when filling a list of Images, and you select 30 gameobject with only 20 having images, it fills the list with references to the 20 image component from the selected items.This tool is incredibly useful to skip having to lock the inspector on the object with the list to fill, then selecting and dragging the components in at once. Has the keybind alt+s.

  - **Vector3**: overridden to provide fill options using the transform fields (world/local position, scale, euler angles).


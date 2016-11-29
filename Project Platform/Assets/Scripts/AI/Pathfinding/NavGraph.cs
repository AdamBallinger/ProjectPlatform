using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.AI.Pathfinding
{
    public class NavGraph
    {

        /// <summary>
        /// 2D node graph array for the world.
        /// </summary>
        public PathNode[,] Nodes { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Creates a new nav graph instance for the given width and height.
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public NavGraph(int _width, int _height)
        {
            Width = _width;
            Height = _height;
            Nodes = new PathNode[Width, Height];

            for(var x = 0; x < Width; x++)
            {
                for(var y = 0; y < Height; y++)
                {
                    Nodes[x, y] = new PathNode(x, y, PathNodeType.None);
                }
            }
        }

        /// <summary>
        /// Scans the world and creates node points and their links.
        /// </summary>
        public void ScanGraph()
        {
            Clear();
            PlaceNodes();
            ComputeNodeLinks();
        }

        /// <summary>
        /// Places
        /// </summary>
        private void PlaceNodes()
        {
            // Store the currently started platforms ID.
            var currentPlatformID = 0;

            for (var y = 0; y < Height; y++)
            {
                // Is there a platform thats already been started. When starting on a new Y level, then there isn't a current platform.
                var hasPlatformStarted = false;

                // Go through each row in the world
                for (var x = 0; x < Width; x++)
                {
                    // If a platform hasn't been started.
                    if (!hasPlatformStarted)
                    {
                        // Check for the first empty tile that has a platform tile under it.
                        if (World.Current.GetTileAt(x, y).Type == TileType.Empty // If target tile is free
                            && World.Current.GetTileAt(x, y - 1) != null && World.Current.GetTileAt(x, y - 1).Type == TileType.Platform) // If tile below is a platform
                        {
                            // new platform started so add left edge node to the tile at the current X and Y.
                            Nodes[x, y].NodeType = PathNodeType.LeftEdge;
                            Nodes[x, y].PlatformID = currentPlatformID;
                            hasPlatformStarted = true;
                        }
                    }

                    // If we have an already started platform.
                    if (hasPlatformStarted)
                    {
                        // Get the tile to the right and down of the current tile.
                        var tileLowerRight = World.Current.GetTileAt(x + 1, y - 1);

                        // Get the tile to the immediate right of the current tile.
                        var tileRight = World.Current.GetTileAt(x + 1, y);

                        // if the lower right tile is not null or a platform, the tile to the right isn't null and empty 
                        // and the current node isn't a left edge node, then the node is a normal platform node.
                        if (tileLowerRight != null && tileLowerRight.Type == TileType.Platform
                            && tileRight != null && tileRight.Type == TileType.Empty
                            && Nodes[x, y].NodeType != PathNodeType.LeftEdge)
                        {
                            Nodes[x, y].NodeType = PathNodeType.Platform;
                            Nodes[x, y].PlatformID = currentPlatformID;
                        }
                        
                        // if the lower right tile is an empty tile OR the tile to the right is a platform
                        // We need to check then if the tile is a single platform node or the right edge of a platform.
                        if ((tileLowerRight != null && tileLowerRight.Type == TileType.Empty)
                            || (tileRight != null && tileRight.Type == TileType.Platform))
                        {
                            // If the current node is a left edge node then change the node to be a single node type.
                            // otherwise set it as a right edge node.
                            Nodes[x, y].NodeType = Nodes[x, y].NodeType == PathNodeType.LeftEdge ? PathNodeType.Single : PathNodeType.RightEdge;

                            // Reached end of the current platform as a single or right edge node was placed.
                            hasPlatformStarted = false;
                            // Increment next platform ID.
                            currentPlatformID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Computes all links for each node in the graph.
        /// </summary>
        private void ComputeNodeLinks()
        {
            ComputeWalkLinks();
            ComputeFallLinks();
            ComputeJumpLinks();
        }

        /// <summary>
        /// Computer the standard walking links between each node in the graph.
        /// </summary>
        private void ComputeWalkLinks()
        {
            for(var y = 0; y < Height; y++)
            {
                for(var x = 0; x < Width; x++)
                {
                    if(Nodes[x, y].NodeType != PathNodeType.None && Nodes[x, y].NodeType != PathNodeType.RightEdge)
                    {
                        if(Nodes[x + 1, y] != null && Nodes[x + 1, y].NodeType != PathNodeType.None)
                        {
                            // Add walk links both ways.
                            Nodes[x, y].AddLink(new NodeLink(Nodes[x + 1, y], NodeLinkType.Walk));
                            Nodes[x + 1, y].AddLink(new NodeLink(Nodes[x, y], NodeLinkType.Walk));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compute the fall links for each node in the graph.
        /// </summary>
        private void ComputeFallLinks()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    // Only generate fall links from LeftEdge / RightEdge / Single node types
                    if(Nodes[x, y].NodeType == PathNodeType.LeftEdge || Nodes[x, y].NodeType == PathNodeType.RightEdge || Nodes[x, y].NodeType == PathNodeType.Single)
                    {
                        // Control variables to determine side of the current tile to create a fall link.
                        // Default of 0 and 0 will be for the tile to the left.
                        var j = 0;
                        var k = 0;

                        switch(Nodes[x, y].NodeType)
                        {
                            case PathNodeType.RightEdge:
                                j = 1; // start from 1 (right tile)
                                k = 1; // end at 1 for the for loop below (right tile)
                                break;

                            case PathNodeType.Single:
                                j = 0; // start at 0 (left tile)
                                k = 1; // end at 1 (both left and right tiles)
                                break;
                        }

                        // Based on j and k values, create the fall links.
                        for(var i = j; i <= k; i++)
                        {
                            // If i is 0 then get the left tile else get the right tile. (from current tile at x and y)
                            var sideTile = i == 0 ? World.Current.GetTileAt(x - 1, y) : World.Current.GetTileAt(x + 1, y);

                            if(sideTile != null && sideTile.Type != TileType.Platform)
                            {
                                var targetY = sideTile.Y - 1;

                                // Until we reach the bottom of the world (y = 0) find the next node that isnt of type none,
                                // and add a fall link to it from the current node
                                while(targetY > 0)
                                {
                                    var nodeToCheck = Nodes[sideTile.X, targetY];

                                    if(nodeToCheck != null && nodeToCheck.NodeType != PathNodeType.None)
                                    {
                                        // Found a node that isnt of type none, so add a fall link to this node from
                                        // current node and break from the loop.
                                        Nodes[x, y].AddLink(new NodeLink(nodeToCheck, NodeLinkType.Fall));
                                        break;
                                    }

                                    // if node type was none, then check the next tile below.
                                    targetY--;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compute the jump links for the AI for each node in the graph.
        /// </summary>
        private void ComputeJumpLinks()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var node = Nodes[x, y];

                    // Only check trajectories for Edge / single and platform nodes with fall links.
                    if(node.NodeType == PathNodeType.LeftEdge || node.NodeType == PathNodeType.RightEdge || node.NodeType == PathNodeType.Single
                        || (node.NodeType == PathNodeType.Platform && node.HasLinkOfType(NodeLinkType.Fall)))
                    {
                        // Computer trajectories here (left and right).
                    }
                }
            }
        }

        /// <summary>
        /// Sets each node in the graph to type none, and clears any links the node previously had.
        /// </summary>
        public void Clear()
        {
            for(var x = 0; x < Width; x++)
            {
                for(var y = 0; y < Height; y++)
                {
                    Nodes[x, y].NodeType = PathNodeType.None;
                    Nodes[x, y].ClearLinks();
                }
            }
        }
    }
}

using System.Collections.Generic;
using BehaviorTree;

public class HumanBTree : BTree
{
    // waypoints to settlements / biomes??????
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node> {
                new CanBuild(),
                new Build(),
                new EndTurn(),
            }),
            new Sequence(new List<Node> {
                new ScanSurroundings(),
                new Selector(new List<Node> {
                    new Fight(),
                    new Conquer(),
                }),
                new EndTurn()
            }), 
            new EndTurn()
        });

        return root;
    }
}

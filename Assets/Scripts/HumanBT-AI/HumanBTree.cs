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
                    // need another scrip for simply wandering in case there's nothing left to conquer -- check for that in the conquer script
                }),
                new EndTurn()
            }),
        });

        return root;
    }
}

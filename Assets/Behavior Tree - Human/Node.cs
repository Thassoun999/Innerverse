using System.Collections;
using System.Collections.Generic;


namespace BehaviorTree
{
    public enum NodeState
    { 
        RUNNING,
        SUCCESS,
        FAILURE
    }
    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        // Data agnostic sharing pattern
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node() {
            parent = null;
        }

        public Node(List<Node> children) 
        {
            foreach (Node child in children) {
                _Attach(child);
            }
        }

        private void _Attach(Node node) {
            node.parent = this;
            children.Add(node);
        }

        // virtual so each derived node class can implement its own evaluation function and have a unique role in the behavior tree
        // Inherited children COULD override and define this (if they want to)
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value) {
            _dataContext[key] = value;
        }

        // We want to check if the data is defined somehwere in our branch and not just in this particular node (make it easier to access and use shared data in the behavior tree)
        public object GetData(string key) {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
            while(node != null) {
                value = node.GetData(key);
                if (value != null)
                    return value;

                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key) {
            if (_dataContext.ContainsKey(key)) {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while(node != null) {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;

                node = node.parent;
            }

            return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        public Transform[] waypoints;
        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        void Update()
        {
            if (_root != null && !GameManager.Instance.PlayerTurn)
                _root.Evaluate();
        }

        // Inherited children NEED to override and define this
        protected abstract Node SetupTree();
    }
}

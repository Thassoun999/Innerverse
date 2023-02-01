using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyceliumGroup {
    // ~ Instances and Variables ~
    
    private static List<Mycelium> _MyceliumGroup;
    private static int _MyceliumCount;

    // ~ Properties ~

    public static List<Mycelium> MyceliumList {
        get {
            return _MyceliumGroup;
        }
    }
}
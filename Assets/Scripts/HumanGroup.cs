using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HumanGroup {
    // ~ Instances and Variables ~
    
    private static List<Human> _HumanGroup;
    private static int _HumanCount;

    // ~ Properties ~

    public static List<Human> HumanList {
        get {
            return _HumanGroup;
        }
    }
}

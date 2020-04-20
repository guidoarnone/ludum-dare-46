using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class Battle {

    protected enum FightState { Tie, A, B, Ongoing }

    Army A;
    Army B;

    Int2 hitA;
    Int2 hitB;

    public Battle (Army A, Army B) {
        this.A = A;
        this.B = B;
        int hits = Math.max(Math.round(Mathf.Log10(Math.min(A, B))), 1);
        int Adamage = Math.clamp(B, 0, A);
        int Bdamage = Math.clamp(A, 0, B);
        hitA = new Int2(Adamage/hits, Adamage%hits);
        hitB = new Int2(Bdamage/hits, Bdamage%hits);
    }

    public int hit() {
        A.remove(hitA.x);
        if (A<=hitA.y) { A.remove(hitA.y); }

        B.remove(hitB.x);
        if (B<=hitB.y) { B.remove(hitB.y); }

        if (A.empty || B.empty) {
            if (!A.empty) { return 2; }
            else if(!B.empty) { return -1; }
            else { return 1; } 
        }
        return 0;
    }
}

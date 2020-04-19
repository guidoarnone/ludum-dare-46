using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class Battle {

    protected enum FightState { Tie, A, B, Ongoing }

    CombatEnd winner;

    Army A;
    Army B;

    Int2 hitA;
    Int2 hitB;

    public Battle (Army A, Army B, CombatEnd winner) {
        this.winner = winner;
        this.A = A;
        this.B = B;
        int hits = Math.round(Mathf.Log10(Math.min(A, B)));
        int Adamage = Math.clamp(B, 0, A);
        int Bdamage = Math.clamp(A, 0, B);
        hitA = new Int2(Adamage/hits, Adamage%hits);
        hitB = new Int2(Bdamage/hits, Bdamage%hits);

        
    }

    public bool hit() {
        A.remove(hitA.x);
        if (A<=hitA.y) { A.remove(hitA.y); }

        B.remove(hitB.x);
        if (B<=hitB.y) { B.remove(hitB.y); }

        if (A.empty || B.empty) {
            if (!A.empty) { winner(A); }
            else if(!B.empty) { winner(B); }
            else { winner(null); }
            return true;
        }
        return false;
    }
}

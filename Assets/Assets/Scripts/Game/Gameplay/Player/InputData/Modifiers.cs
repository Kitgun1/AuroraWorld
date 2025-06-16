using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.Player.InputData
{
    public struct Modifiers : IEquatable<Modifiers>
    {
        public bool HasCapsLock;
        public bool HasCtrl;
        public bool HasShift;
        public bool HasAlt;

        public bool OnlyCapsLock => HasCapsLock && !HasCtrl && !HasShift && !HasAlt;
        public bool OnlyCtrl => !HasCapsLock && HasCtrl && !HasShift && !HasAlt;
        public bool OnlyShift => !HasCapsLock && !HasCtrl && HasShift && !HasAlt;
        public bool OnlyAlt => !HasCapsLock && !HasCtrl && !HasShift && HasAlt;
        public bool All => HasCapsLock && HasCtrl && HasShift && HasAlt;
        public bool Any => HasCapsLock || HasCtrl || HasShift || HasAlt;

        public override bool Equals(object obj) => base.Equals(obj);

        public bool Equals(Modifiers other) => HasCapsLock == other.HasCapsLock && HasCtrl == other.HasCtrl &&
                                               HasShift == other.HasShift && HasAlt == other.HasAlt;

        public override int GetHashCode() => HashCode.Combine(HasCapsLock, HasCtrl, HasShift, HasAlt);
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Soundboard.Clips
{
    public struct SoundInfo : IEquatable<SoundInfo>
    {
        public bool Enabled { get; set; }

        public string Path { get; set; }

        public float Start { get; set; }
        public float Length { get; set; }

        public float VolumeModifier { get; set; }
        public bool Cached { get; set; }

        public PlayBehavior Behavior { get; set; }
        public bool SinglePlay { get; set; }
        public bool SinglePlayImmunity { get; set; }

        public bool Equals([AllowNull] SoundInfo other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Enabled != other.Enabled)
                return false;
            if (Path != other.Path)
                return false;
            if (Start != other.Start)
                return false;
            if (Length != other.Length)
                return false;
            if (VolumeModifier != other.VolumeModifier)
                return false;
            if (Cached != other.Cached)
                return false;
            if (Behavior != other.Behavior)
                return false;
            if (SinglePlay != other.SinglePlay)
                return false;
            if (SinglePlayImmunity != other.SinglePlayImmunity)
                return false;
            return true;
        }

        public enum PlayBehavior
        {
            Restart,
            Stop,
            StartNew
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return Equals((SoundInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ Enabled.GetHashCode();
                hash = (hash * HashingMultiplier) ^ Path?.GetHashCode() ?? 0;
                hash = (hash * HashingMultiplier) ^ Start.GetHashCode();
                hash = (hash * HashingMultiplier) ^ Length.GetHashCode();
                hash = (hash * HashingMultiplier) ^ VolumeModifier.GetHashCode();
                hash = (hash * HashingMultiplier) ^ Cached.GetHashCode();
                hash = (hash * HashingMultiplier) ^ Behavior.GetHashCode();
                hash = (hash * HashingMultiplier) ^ SinglePlay.GetHashCode();
                hash = (hash * HashingMultiplier) ^ SinglePlayImmunity.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(SoundInfo left, SoundInfo right) => left.Equals(right);

        public static bool operator !=(SoundInfo left, SoundInfo right) => !(left == right);
    }
}

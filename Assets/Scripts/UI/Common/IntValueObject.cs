namespace UI.Common
{
    public class IntValueObject
    {
        protected bool Equals(IntValueObject other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntValueObject) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(IntValueObject left, IntValueObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntValueObject left, IntValueObject right)
        {
            return !Equals(left, right);
        }

        public int Value { get; private set; }

        public IntValueObject(int value)
        {
            Value = value;
        }
    }
}
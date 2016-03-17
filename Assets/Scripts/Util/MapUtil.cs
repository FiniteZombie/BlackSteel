using UnityEngine;

namespace Assets.Scripts.MapUtil
{
    public class Pair<X, Y>
    {
        private X _x;
        private Y _y;

        public Pair(X first, Y second)
        {
            _x = first;
            _y = second;
        }

        public X first { get { return _x; } }

        public Y second { get { return _y; } }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            Pair<X, Y> other = obj as Pair<X, Y>;
            if (other == null)
                return false;

            return
                (((first == null) && (other.first == null))
                    || ((first != null) && first.Equals(other.first)))
                    &&
                (((second == null) && (other.second == null))
                    || ((second != null) && second.Equals(other.second)));
        }

        public override int GetHashCode()
        {
            int hashcode = 0;
            if (first != null)
                hashcode += first.GetHashCode();
            if (second != null)
                hashcode += second.GetHashCode();

            return hashcode;
        }

        public override string ToString()
        {
            return "<" + _x + ", " + _y + ">";
        }
    }

    public class Edge<X, Y> : Pair<X, Y>
    {
        private X _x;
        private Y _y;

        public Edge(X first, Y second)
            : base(first, second)
        { }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            Edge<X, Y> other = obj as Edge<X, Y>;
            if (other == null)
                return false;

            return
                (
                    (((first == null) && (other.first == null))
                        || ((first != null) && first.Equals(other.first)))
                      &&
                    (((second == null) && (other.second == null))
                        || ((second != null) && second.Equals(other.second)))
                )
                ||
                (
                    (((first == null) && (other.second == null))
                        || ((first != null) && first.Equals(other.second)))
                      &&
                    (((second == null) && (other.first == null))
                        || ((second != null) && second.Equals(other.first)))
                );
        }
    }

    public class Edge2 : Edge<Vector2, Vector2>
    {
        public Edge2(Vector2 first, Vector2 second)
            : base(first, second)
        { }
    }

    public class Edge3 : Edge<Vector3, Vector3>
    {
        public Edge3(Vector3 first, Vector3 second)
            : base(first, second)
        { }
    }

    class MapUtil
    {
    }
}

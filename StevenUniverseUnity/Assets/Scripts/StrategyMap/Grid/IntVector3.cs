using UnityEngine;
using System.Collections;
using System;

namespace StevenUniverse.FanGame.StrategyMap
{
	[System.Serializable]
	public struct IntVector3 : IEquatable<IntVector3>
	{
		public int x, y, z;
		
		public IntVector3( int x, int y, int z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public IntVector3( Vector3 v )
		{
			this.x = Mathf.FloorToInt(v.x); this.y = Mathf.FloorToInt(v.y); this.z = Mathf.FloorToInt(v.z);
		}

		public IntVector3( Vector2 v ) : this( (Vector3)v ) {}

		public static explicit operator IntVector3( Vector2 v )
		{
			return new IntVector3( v );
		}

        public static explicit operator Vector2( IntVector3 v )
        {
            return new Vector2(v.x, v.y);
        }
		
		public static explicit operator Vector3( IntVector3 v )
		{
			return new Vector3( v.x, v.y, v.z );
		}
		
		public static explicit operator IntVector3( Vector3 v )
		{
			return new IntVector3( v );
		}

        public static explicit operator IntVector3(IntVector2 v)
        {
            return new IntVector3(v.x, v.y, 0);
        }


        // Static Properties
        public static IntVector3 forward { get { return new IntVector3 (0, 0, 1); } }
		public static IntVector3 back { get { return new IntVector3 (0, 0, -1); } }
		public static IntVector3 up { get { return new IntVector3 (0, 1, 0 ); } }
		public static IntVector3 down { get { return new IntVector3 (0, -1, 0); } }
		public static IntVector3 right { get { return new IntVector3 (1, 0, 0); } }
		public static IntVector3 left { get { return new IntVector3 (-1, 0, 0); } }
		public static IntVector3 one { get { return new IntVector3 ( 1, 1, 1); } }
		public static IntVector3 zero { get { return new IntVector3 ( 0, 0, 0 ); } }
		
		public string ToString (string format)
		{
			return string.Format ("({0}, {1}, {2})", new object[]
			{
				this.x.ToString (format),
				this.y.ToString (format),
				this.z.ToString (format)
			});
		}
		
		public override string ToString ()
		{
			return string.Format ("({0:F1}, {1:F1}, {2:F1})", new object[]
			{
				this.x,
				this.y,
				this.z
			});
		}

		public static IntVector3 operator + (IntVector3 a, IntVector3 b)
		{
			return new IntVector3 (a.x + b.x, a.y + b.y, a.z + b.z);
		}
		
		
		public static IntVector3 operator - (IntVector3 a, IntVector3 b)
		{
			return new IntVector3 (a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static IntVector3 operator / ( IntVector3 a, int i )
		{
			return new IntVector3 ( a.x / i, a.y / i, a.z / i );
		}

		public static IntVector3 operator * ( IntVector3 v, int i )
		{
			return new IntVector3( v.x * i, v.y * i, v.z * i );
		}
		
		public static bool operator == (IntVector3 lhs, IntVector3 rhs)
		{
			return IntVector3.SqrMagnitude (lhs - rhs) == 0;
		}

		public static IntVector3 Scale (IntVector3 a, IntVector3 b)
		{
			return new IntVector3 (a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public void Scale (IntVector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}
		
		public static int SqrMagnitude (IntVector3 a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z;
		}
		
		public static IntVector3 operator - ( IntVector3 a)
		{
			return new IntVector3 (-a.x, -a.y, -a.z);
		}
		
		public static bool operator != (IntVector3 lhs, IntVector3 rhs)
		{
			return IntVector3.SqrMagnitude (lhs - rhs) != 0;
		}
		
		public override bool Equals (object other)
		{
			if (!(other is IntVector3))
			{
				return false;
			}
			IntVector3 vector = (IntVector3)other;
			return this.x.Equals (vector.x) && this.y.Equals (vector.y) && this.z.Equals (vector.z);
		}

        // IEquatable ( Prevents allocations if we use this in dictionaries/hashtables)
        public bool Equals(IntVector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override int GetHashCode ()
		{
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + x;
                hash = hash * 23 + y;
                hash = hash * 23 + z;
                return hash;
            }
		}

		//
		// Indexer
		//
		public int this [int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
					throw new IndexOutOfRangeException ("Invalid IVector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				default:
					throw new IndexOutOfRangeException ("Invalid IVector3 index!");
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			yield return x;
			yield return y;
			yield return z;

			yield return false;
		}


    }
}
